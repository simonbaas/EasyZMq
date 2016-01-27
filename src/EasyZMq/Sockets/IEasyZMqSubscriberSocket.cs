using System;

namespace EasyZMq.Sockets
{
    public interface IEasyZMqSubscriberSocket : IDisposable
    {
        Uri Uri { get; }
        void Start();
        IDisposable On<T>(Action<T> action);
    }
}