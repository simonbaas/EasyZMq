using System;

namespace EasyZMq.Sockets
{
    public interface ISocket : IDisposable
    {
        Uri Uri { get; }
    }
}