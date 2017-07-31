using System;
using EasyZMq.Infrastructure;

namespace EasyZMq.Sockets.Subscriber
{
    public interface IDynamicSubscriberSocket : IStartableSocket, IMonitorConnection, IDisposable
    {
        Uri Uri { get; }
        IDisposable On(Action<dynamic> action);
    }
}
