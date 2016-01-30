using System;

namespace EasyZMq.Sockets.Subscriber
{
    public interface ISubscriberSocket : IDisposable
    {
        Uri Uri { get; }
        void Start();
        IDisposable On<T>(Action<T> action);
    }
}