using System;
using NetMQ;

namespace EasyZMq.Infrastructure
{
    internal class BindAddressBinder : IAddressBinder
    {
        public Uri Uri { get; }

        public BindAddressBinder(Uri uri)
        {
            Uri = uri;
        }

        public void ConnectOrBindAddress(NetMQSocket socket)
        {
            var address = $"{Uri.Scheme}://{Uri.Host}:{Uri.Port}";
            socket.Bind(address);
        }
    }
}
