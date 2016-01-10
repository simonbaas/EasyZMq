using System;
using EasyZMq.Configuration;
using EasyZMq.Infrastructure;
using NetMQ;

namespace EasyZMq.Sockets
{
    public class EasyZMqSubscriberSocket : EasyZMqReceiverSocket, IEasyZMqSubscriberSocket
    {
        private IMessageDispatcher _dispatcher;

        public EasyZMqSubscriberSocket(EasyZMqConfiguration configuration, NetMQContext context, NetMQSocket socket)
            : base(configuration, context, socket)
        {
            _dispatcher = new MessageDispatcher(configuration);
        }

        public IDisposable On<T>(Action<T> action)
        {
            var subscription = _dispatcher.Subscribe<T>();

            Action<dynamic> handler = message =>
            {
                action((T)message);
            };

            subscription.Received += handler;

            return new DisposableAction(() => subscription.Received -= handler);
        }

        public override void OnMessageReceived<T>(T message)
        {
            _dispatcher.Dispatch(message);
        }

        private bool _disposedValue;
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (_dispatcher != null)
                    {
                        _dispatcher.Dispose();
                        _dispatcher = null;
                    }
                }

                _disposedValue = true;
            }
        }
    }
}