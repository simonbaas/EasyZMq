using System;
using NetMQ;
using NetMQ.Monitoring;

namespace EasyZMq.Infrastructure
{
    internal class ConnectionMonitor : IConnectionMonitor
    {
        public event Action Connected;
        public event Action Disconnected;
        public event Action ConnectRetried;

        private NetMQMonitor _monitor;

        public ConnectionMonitor(NetMQContext context, NetMQSocket monitoredSocket, Poller poller)
        {
            _monitor = CreateMonitor(context, monitoredSocket, poller);
        }

        private NetMQMonitor CreateMonitor(NetMQContext context, NetMQSocket socket, Poller poller)
        {
            var monitor = new NetMQMonitor(context, socket, $"inproc://{Guid.NewGuid()}.inproc",
                SocketEvents.Connected | SocketEvents.Disconnected | SocketEvents.ConnectRetried);

            monitor.Connected += Monitor_Connected;
            monitor.Disconnected += Monitor_Disconnected;
            monitor.ConnectRetried += Monitor_ConnectRetried;

            monitor.AttachToPoller(poller);

            return monitor;
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
                    if (_monitor != null)
                    {
                        DisposeMonitor();
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
    }
}
