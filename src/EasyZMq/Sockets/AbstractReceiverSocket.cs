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
        private NetMQSocket _socket;
        private NetMQPoller _poller;

        public event Action Connected;
        public event Action Disconnected;
        public event Action ConnectRetried;

        protected AbstractReceiverSocket(ISerializer serializer, IAddressBinder addressBinder, ILoggerFactory loggerFactory, NetMQSocket socket)
        {
            _logger = loggerFactory.GetLogger(typeof (AbstractReceiverSocket));
            _serializer = serializer;
            _addressBinder = addressBinder;
            _socket = socket;

            CreatePoller(socket);
            CreateConnectionMonitor(socket, _poller);
            ConfigureSocket(socket);
        }

        public Uri Uri => _addressBinder.Uri;

        public void Start()
        {
            if (_disposedValue) throw new ObjectDisposedException("EasyZMqReceiverSocket");

            _addressBinder.ConnectOrBindAddress(_socket);

            //_poller.PollTillCancelledNonBlocking();
            _poller.Run();
        }

        public abstract void OnMessageReceived<T>(T message);

        private void CreatePoller(ISocketPollable socket)
        {
            _poller = new NetMQPoller() { socket };
        }

        private void CreateConnectionMonitor(NetMQSocket socket, NetMQPoller poller)
        {
            _connectionMonitor = new ConnectionMonitor(socket, poller);
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
                        if (_poller.IsRunning)
                        {
                            _poller.Stop();
                        }

                        DisposeConnectionMonitor();
                        DisposePoller();
                        DisposeSocket();
                    }

                    NetMQConfig.Cleanup();
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
            _poller.Stop();
            _poller.Dispose();
            _poller = null;
        }

        private void DisposeSocket()
        {
            _socket.ReceiveReady -= Subscriber_ReceiveReady;
            _socket.Dispose();
            _socket = null;
        }
    }
}
