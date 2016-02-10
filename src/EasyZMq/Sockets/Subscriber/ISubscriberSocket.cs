using EasyZMq.Infrastructure;
using System;

namespace EasyZMq.Sockets.Subscriber
{
    public interface ISubscriberSocket : IStartableSocket, ISocket, IMonitorConnection
    {
        IDisposable On<T>(Action<T> action);
    }
}