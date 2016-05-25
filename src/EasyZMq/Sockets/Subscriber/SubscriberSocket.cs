using System;
using EasyZMq.Infrastructure;
using EasyZMq.Logging;
using EasyZMq.Serialization;
using NetMQ;

namespace EasyZMq.Sockets.Subscriber
{
    internal class SubscriberSocket : AbstractReceiverSocket, ISubscriberSocket, IDynamicSubscriberSocket
    {
        private IMessageDispatcher _messageDispatcher;

        public SubscriberSocket(ISerializer serializer, IAddressBinder addressBinder, ILoggerFactory loggerFactory, IMessageDispatcher messageDispatcher,
            NetMQContext context, NetMQSocket socket)
            : base(serializer, addressBinder, loggerFactory, context, socket)
        {
            _messageDispatcher = messageDispatcher;
        }

        public IDisposable On<T>(Action<T> action)
        {
            var subscription = _messageDispatcher.Subscribe<T>();

            Action<dynamic> handler = message =>
            {
                action((T)message);
            };

            subscription.Received += handler;

            return new DisposableAction(() => subscription.Received -= handler);
        }

        public IDisposable On(Action<dynamic> action)
        {
            return On<dynamic>(action);
        }

        public override void OnMessageReceived<T>(T message)
        {
            _messageDispatcher.Dispatch(message);
        }

        private bool _disposedValue;
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (_messageDispatcher != null)
                    {
                        _messageDispatcher.Dispose();
                        _messageDispatcher = null;
                    }
                }

                _disposedValue = true;
            }
        }
    }
}