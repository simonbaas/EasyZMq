using System;
using NetMQ;

namespace EasyZMq.Infrastructure
{
    internal interface IAddressBinder
    {
        Uri Uri { get; }
        void ConnectOrBindAddress(NetMQSocket socket);
    }
}
