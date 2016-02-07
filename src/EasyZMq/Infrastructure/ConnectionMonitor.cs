using System;
using EasyZMq.Logging;
using NetMQ;
using NetMQ.Monitoring;

namespace EasyZMq.Infrastructure
{
    internal class ConnectionMonitor : IConnectionMonitor
    {
        public event Action Connected;
        public event Action Disconnected;
        public event Action ConnectRetried;

        private readonly ILogger _logger;
        private NetMQMonitor _monitor;

        public ConnectionMonitor(ILoggerFactory loggerFactory, NetMQContext context, NetMQSocket monitoredSocket, Poller poller)
        {
            _logger = loggerFactory.GetLogger(typeof (ConnectionMonitor));
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
            try
            {
                handler?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Error("Event handler for Connected event threw an unhandled exception", ex);
            }
        }

        private void Monitor_Disconnected(object sender, NetMQMonitorSocketEventArgs e)
        {
            var handler = Disconnected;
            try
            {
                handler?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Error("Event handler for Disconnected event threw an unhandled exception", ex);
            }
        }

        private void Monitor_ConnectRetried(object sender, NetMQMonitorIntervalEventArgs e)
        {
            var handler = ConnectRetried;
            try
            {
                handler?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Error("Event handler for ConnectRetried event threw an unhandled exception", ex);
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
