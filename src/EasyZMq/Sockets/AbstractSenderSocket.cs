using System;
using EasyZMq.Configuration;
using EasyZMq.Infrastructure;
using EasyZMq.Serialization;
using NetMQ;

namespace EasyZMq.Sockets
{
    public abstract class AbstractSenderSocket : ISenderSocket
    {
        private readonly ISerializer _serializer;
        private readonly IAddressBinder _addressBinder;
        private readonly NetMQContext _context;
        private readonly NetMQSocket _socket;

        protected AbstractSenderSocket(ISerializer serializer, IAddressBinder addressBinder, NetMQContext context, NetMQSocket socket)
        {
            _serializer = serializer;
            _addressBinder = addressBinder;
            _context = context;
            _socket = socket;

            _addressBinder.ConnectOrBindAddress(socket);
        }

        public Uri Uri => _addressBinder.Uri;

        public void SendMessage<T>(string topic, T message)
        {
            var serializedMessage = _serializer.Serialize(message);

            _socket.SendMoreFrame(topic).SendFrame(serializedMessage);
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
                    _socket.Dispose();
                    _context.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
