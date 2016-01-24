using System;
using EasyZMq.Configuration;
using EasyZMq.Logging;
using NetMQ;
using NetMQ.Monitoring;

namespace EasyZMq.Sockets
{
    public abstract class EasyZMqReceiverSocket : IEasyZMqReceiverSocket
    {
        private readonly EasyZMqConfiguration _configuration;
        private readonly ILogger _logger;
        private NetMQContext _context;
        private NetMQSocket _socket;
        private NetMQMonitor _monitor;
        private Poller _poller;

        public event Action Connected;
        public event Action Disconnected;
        public event Action ConnectRetried;

        protected EasyZMqReceiverSocket(EasyZMqConfiguration configuration, NetMQContext context, NetMQSocket socket)
        {
            _configuration = configuration;
            _logger = configuration.EasyZMqLoggerFactory.GetLogger(typeof (EasyZMqReceiverSocket));
            _context = context;
            _socket = socket;

            CreatePoller(socket);
            CreateMonitor(context, socket, _poller);
            ConfigureSocket(socket);
        }

        public Uri Uri => _configuration.AddressBinder.Uri;

        public void Start()
        {
            if (_disposedValue) throw new ObjectDisposedException("EasyZMqReceiverSocket");

            _configuration.AddressBinder.ConnectOrBindAddress(_socket);

            _poller.PollTillCancelledNonBlocking();
        }

        public abstract void OnMessageReceived<T>(T message);

        private void CreatePoller(ISocketPollable socket)
        {
            _poller = new Poller(socket);
        }

        private void CreateMonitor(NetMQContext context, NetMQSocket socket, Poller poller)
        {
            _monitor = new NetMQMonitor(context, socket, $"inproc://{Guid.NewGuid()}.inproc",
                SocketEvents.Connected | SocketEvents.Disconnected | SocketEvents.ConnectRetried);

            _monitor.Connected += Monitor_Connected;
            _monitor.Disconnected += Monitor_Disconnected;
            _monitor.ConnectRetried += Monitor_ConnectRetried;

            _monitor.AttachToPoller(poller);
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

        private void Monitor_Connected(object sender, NetMQMonitorSocketEventArgs e)
        {
            var handler = Connected;
            handler?.Invoke();
        }

        private void Monitor_Disconnected(object sender, NetMQMonitorSocketEventArgs e)
        {
            var handler = Disconnected;
            handler?.Invoke();
        }

        private void Monitor_ConnectRetried(object sender, NetMQMonitorIntervalEventArgs e)
        {
            var handler = ConnectRetried;
            handler?.Invoke();
        }

        private void DeserializeAndDispatch(byte[] bytes)
        {
            try
            {
                dynamic message = _configuration.Serializer.Deserialize(bytes);
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

                        DisposeMonitor();
                        DisposePoller();
                        DisposeSocket();
                        DisposeContext();
                    }
                }

                _disposedValue = true;
            }
        }

        private void DisposeMonitor()
        {
            _monitor.Connected -= Monitor_Connected;
            _monitor.Disconnected -= Monitor_Disconnected;
            _monitor.ConnectRetried -= Monitor_ConnectRetried;
            _monitor.Dispose();
            _monitor = null;
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
