using System;
using NetMQ;

namespace EasyZMq.Infrastructure
{
    public class BindRandomPortAddressBinder : IAddressBinder
    {
        public Uri Uri { get; private set; }

        public BindRandomPortAddressBinder(string address)
        {
            Uri = new Uri(address);
        }

        public void ConnectOrBindAddress(NetMQSocket socket)
        {
            var port = socket.BindRandomPort(Uri.AbsoluteUri.TrimEnd('/'));
            var address = string.Format("{0}://{1}:{2}", Uri.Scheme, Uri.Host, port);

            Uri = new Uri(address);
        }
    }
}