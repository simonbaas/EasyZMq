using System;
using NetMQ;

namespace EasyZMq.Infrastructure
{
    internal class ConnectAddressBinder : IAddressBinder
    {
        public Uri Uri { get; }

        public ConnectAddressBinder(Uri uri)
        {
            Uri = uri;
        }

        public void ConnectOrBindAddress(NetMQSocket socket)
        {
            var address = $"{Uri.Scheme}://{Uri.Host}:{Uri.Port}";
            socket.Connect(address);
        }
    }
}
