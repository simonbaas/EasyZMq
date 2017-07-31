using System;

namespace EasyZMq.Sockets
{
    internal interface IReceiverSocket : IStartableSocket, IDisposable
    {
        Uri Uri { get; }
        void OnMessageReceived<T>(T message);
    }
}