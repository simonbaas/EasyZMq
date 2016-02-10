using System;
using EasyZMq.Infrastructure;
using EasyZMq.Logging;
using EasyZMq.Serialization;
using NetMQ;

namespace EasyZMq.Sockets
{
    internal abstract class AbstractReceiverSocket : IReceiverSocket, IMonitorConnection
    {
        private readonly ILogger _logger;
        private readonly ISerializer _serializer;
        private readonly IAddressBinder _addressBinder;
        private IConnectionMonitor _connectionMonitor;
        private NetMQContext _context;
        private NetMQSocket _socket;
        private Poller _poller;

        public event Action Connected;
        public event Action Disconnected;
        public event Action ConnectRetried;

        protected AbstractReceiverSocket(ISerializer serializer, IAddressBinder addressBinder, ILoggerFactory loggerFactory, NetMQContext context, NetMQSocket socket)
        {
            _logger = loggerFactory.GetLogger(typeof (AbstractReceiverSocket));
            _serializer = serializer;
            _addressBinder = addressBinder;
            _context = context;
            _socket = socket;

            CreatePoller(socket);
            CreateConnectionMonitor(context, socket, _poller);
            ConfigureSocket(socket);
        }

        public Uri Uri => _addressBinder.Uri;

        public void Start()
        {
            if (_disposedValue) throw new ObjectDisposedException("EasyZMqReceiverSocket");

            _addressBinder.ConnectOrBindAddress(_socket);

            _poller.PollTillCancelledNonBlocking();
        }

        public abstract void OnMessageReceived<T>(T message);

        private void CreatePoller(ISocketPollable socket)
        {
            _poller = new Poller(socket);
        }

        private void CreateConnectionMonitor(NetMQContext context, NetMQSocket socket, Poller poller)
        {
            _connectionMonitor = new ConnectionMonitor(context, socket, poller);
            _connectionMonitor.Connected += ConnectionMonitor_Connected;
            _connectionMonitor.ConnectRetried += ConnectionMonitor_ConnectRetried;
            _connectionMonitor.Disconnected += ConnectionMonitor_Disconnected;
        }

        private void ConnectionMonitor_Connected()
        {
            try
            {
                var handler = Connected;
                handler?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Error("Event handler for Connected event threw an unhandled exception", ex);
            }
        }

        private void ConnectionMonitor_ConnectRetried()
        {
            try
            {
                var handler = ConnectRetried;
                handler?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Error("Event handler for Disconnected event threw an unhandled exception", ex);
            }
        }

        private void ConnectionMonitor_Disconnected()
        {
            try
            {
                var handler = Disconnected;
                handler?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Error("Event handler for ConnectRetried event threw an unhandled exception", ex);
            }
        }

        private void ConfigureSocket(NetMQSocket socket)
        {
            socket.Options.TcpKeepalive = true;
            socket.Options.TcpKeepaliveIdle = TimeSpan.FromSeconds(5);
            socket.Options.TcpKeepaliveInterval = TimeSpan.FromSeconds(1);
            socket.ReceiveReady += Subscriber_ReceiveReady;
        }

        private void Subscriber_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            bool more;
            e.Socket.ReceiveFrameString(out more);

            if (!more)
            {
                _logger.Warning("Invalid message received! The message was discarded.");
                return;
            }

            var bytes = e.Socket.ReceiveFrameBytes();

            DeserializeAndDispatch(bytes);
        }

        private void DeserializeAndDispatch(byte[] bytes)
        {
            try
            {
                dynamic message = _serializer.Deserialize(bytes);
                OnMessageReceived(message);
            }
            catch (Exception ex)
            {
                _logger.Error("Deserialization and dispatch of incoming message failed.", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_poller != null)
                    {
                        _poller.CancelAndJoin();

                        DisposeConnectionMonitor();
                        DisposePoller();
                        DisposeSocket();
                        DisposeContext();
                    }
                }

                _disposedValue = true;
            }
        }

        private void DisposeConnectionMonitor()
        {
            _connectionMonitor.Connected -= ConnectionMonitor_Disconnected;
            _connectionMonitor.ConnectRetried -= ConnectionMonitor_ConnectRetried;
            _connectionMonitor.Disconnected -= ConnectionMonitor_Disconnected;
            _connectionMonitor.Dispose();
            _connectionMonitor = null;
        }

        private void DisposePoller()
        {
            _poller.Dispose();
            _poller = null;
        }

        private void DisposeSocket()
        {
            _socket.ReceiveReady -= Subscriber_ReceiveReady;
            _socket.Dispose();
            _socket = null;
        }

        private void DisposeContext()
        {
            _context.Dispose();
            _context = null;
        }
    }
}
