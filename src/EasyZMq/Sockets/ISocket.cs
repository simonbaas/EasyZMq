using System;

namespace EasyZMq.Sockets
{
    internal interface ISocket : IDisposable
    {
        Uri Uri { get; }
    }
}