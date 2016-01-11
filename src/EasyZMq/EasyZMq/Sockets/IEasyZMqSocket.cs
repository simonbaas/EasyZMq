using System;

namespace EasyZMq.Sockets
{
    public interface IEasyZMqSocket : IDisposable
    {
        Uri Uri { get; }
    }
}