using System;
using NetMQ;

namespace EasyZMq.Infrastructure
{
    public class ConnectAddressBinder : IAddressBinder
    {
        public Uri Uri { get; }

        public ConnectAddressBinder(Uri uri)
        {
            Uri = uri;
        }

        public void ConnectOrBindAddress(NetMQSocket socket)
        {
            var address = string.Format("{0}://{1}:{2}", Uri.Scheme, Uri.Host, Uri.Port);
            socket.Connect(address);
        }
    }
}
