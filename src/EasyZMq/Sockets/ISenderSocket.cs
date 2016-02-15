using System;

namespace EasyZMq.Sockets
{
    internal interface ISenderSocket : IDisposable
    {
        Uri Uri { get; }
        void SendMessage<T>(string topic, T message);
    }
}