using System;
using NetMQ;

namespace EasyZMq.Infrastructure
{
    public class BindAddressBinder : IAddressBinder
    {
        public Uri Uri { get; }

        public BindAddressBinder(Uri uri)
        {
            Uri = uri;
        }

        public void ConnectOrBindAddress(NetMQSocket socket)
        {
            var address = string.Format("{0}://{1}:{2}", Uri.Scheme, Uri.Host, Uri.Port);
            socket.Bind(address);
        }
    }
}
