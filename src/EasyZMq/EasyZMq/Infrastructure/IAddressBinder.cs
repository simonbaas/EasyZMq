using System;
using NetMQ;

namespace EasyZMq.Infrastructure
{
    public interface IAddressBinder
    {
        Uri Uri { get; }
        void ConnectOrBindAddress(NetMQSocket socket);
    }
}
