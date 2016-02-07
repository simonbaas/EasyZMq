using System;
using NetMQ;

namespace EasyZMq.Infrastructure
{
    internal class BindRandomPortAddressBinder : IAddressBinder
    {
        public Uri Uri { get; private set; }

        public BindRandomPortAddressBinder(string address)
        {
            Uri = new Uri(address);
        }

        public void ConnectOrBindAddress(NetMQSocket socket)
        {
            var port = socket.BindRandomPort(Uri.AbsoluteUri.TrimEnd('/'));
            var address = $"{Uri.Scheme}://{Uri.Host}:{port}";

            Uri = new Uri(address);
        }
    }
}