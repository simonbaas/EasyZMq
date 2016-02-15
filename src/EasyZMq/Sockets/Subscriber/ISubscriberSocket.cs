using EasyZMq.Infrastructure;
using System;

namespace EasyZMq.Sockets.Subscriber
{
    public interface ISubscriberSocket : IStartableSocket, IMonitorConnection, IDisposable
    {
        Uri Uri { get; }
        IDisposable On<T>(Action<T> action);
    }
}