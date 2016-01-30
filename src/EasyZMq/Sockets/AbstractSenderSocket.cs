using System;
using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets
{
    public abstract class AbstractSenderSocket : ISenderSocket
    {
        private readonly EasyZMqConfiguration _configuration;
        private readonly NetMQContext _context;
        private readonly NetMQSocket _socket;

        protected AbstractSenderSocket(EasyZMqConfiguration configuration, NetMQContext context, NetMQSocket socket)
        {
            _configuration = configuration;
            _context = context;
            _socket = socket;

            _configuration.AddressBinder.ConnectOrBindAddress(socket);
        }

        public Uri Uri => _configuration.AddressBinder.Uri;

        public void SendMessage<T>(string topic, T message)
        {
            var serializedMessage = _configuration.Serializer.Serialize(message);

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
