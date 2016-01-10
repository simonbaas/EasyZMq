using System;
using System.Threading.Tasks;
using EasyZMq.Configuration;
using NetMQ;
using NetMQ.Monitoring;

namespace EasyZMq.Sockets
{
    public abstract class EasyZMqReceiverSocket : IEasyZMqReceiverSocket
    {
        private readonly EasyZMqConfiguration _configuration;
        private readonly NetMQContext _context;
        private readonly NetMQSocket _socket;
        private Poller _poller;
        private Task _task;

        public event Action Connected;
        public event Action Disconnected;
        public event Action ConnectRetried;

        protected EasyZMqReceiverSocket(EasyZMqConfiguration configuration, NetMQContext context, NetMQSocket socket)
        {
            _configuration = configuration;
            _context = context;
            _socket = socket;
        }

        public void Start()
        {
            InternalStart();
        }

        public abstract void OnMessageReceived<T>(T message);

        protected void InternalStart()
        {
            _task = Task.Factory.StartNew(() =>
            {
                using (var monitor = new NetMQMonitor(_context, _socket, string.Format("inproc://{0}.inproc", Guid.NewGuid()),
                    SocketEvents.Connected | SocketEvents.Disconnected | SocketEvents.ConnectRetried))
                {
                    _poller = new Poller(_socket);

                    monitor.Connected += Monitor_Connected;
                    monitor.Disconnected += Monitor_Disconnected;
                    monitor.ConnectRetried += Monitor_ConnectRetried;
                    monitor.AttachToPoller(_poller);

                    _socket.Options.TcpKeepalive = true;
                    _socket.Options.TcpKeepaliveIdle = TimeSpan.FromSeconds(5);
                    _socket.Options.TcpKeepaliveInterval = TimeSpan.FromSeconds(1);
                    _socket.ReceiveReady += Subscriber_ReceiveReady;

                    _configuration.AddressBinder.ConnectOrBindAddress(_socket);
                    _poller.PollTillCancelled();

                    _socket.ReceiveReady -= Subscriber_ReceiveReady;
                    monitor.Connected -= Monitor_Connected;
                    monitor.Disconnected -= Monitor_Disconnected;
                    monitor.ConnectRetried -= Monitor_ConnectRetried;
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void Subscriber_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            bool more;
            var data = e.Socket.ReceiveFrameString(out more);
            if (more) data = e.Socket.ReceiveFrameString();

            DeserializeAndDispatch(data);
        }

        private void Monitor_Connected(object sender, NetMQMonitorSocketEventArgs e)
        {
            var handler = Connected;
            if (handler != null)
            {
                handler();
            }
        }

        private void Monitor_Disconnected(object sender, NetMQMonitorSocketEventArgs e)
        {
            var handler = Disconnected;
            if (handler != null)
            {
                handler();
            }
        }

        private void Monitor_ConnectRetried(object sender, NetMQMonitorIntervalEventArgs e)
        {
            var handler = ConnectRetried;
            if (handler != null)
            {
                handler();
            }
        }

        private void DeserializeAndDispatch(string data)
        {
            try
            {
                var message = _configuration.Serializer.Deserialize<dynamic>(data);
                OnMessageReceived(message);
            }
            catch (Exception ex)
            {
                // TODO: Log
            }
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_poller != null)
                    {
                        _poller.CancelAndJoin();
                        _task.Wait();
                        _task.Dispose();
                        _task = null;

                        _poller.Dispose();
                        _poller = null;

                        _socket.Dispose();
                        _context.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}