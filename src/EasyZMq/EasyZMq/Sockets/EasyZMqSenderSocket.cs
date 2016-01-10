using System;
using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets
{
    public abstract class EasyZMqSenderSocket : IEasyZMqSenderSocket
    {
        private readonly EasyZMqConfiguration _configuration;
        private readonly NetMQContext _context;
        private readonly NetMQSocket _socket;

        protected EasyZMqSenderSocket(EasyZMqConfiguration configuration, NetMQContext context, NetMQSocket socket)
        {
            _configuration = configuration;
            _context = context;
            _socket = socket;

            _configuration.AddressBinder.ConnectOrBindAddress(socket);
        }

        public void SendMessage<T>(string topic, T message)
        {
            var serializedMessage = _configuration.Serializer.Serialize(message);

            _socket.SendMoreFrame(topic).SendFrame(serializedMessage);
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _socket.Dispose();
                    _context.Dispose();
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
