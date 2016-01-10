using System;

namespace EasyZMq.Sockets
{
    public interface IEasyZMqSubscriberSocket : IDisposable
    {
        void Start();
        IDisposable On<T>(Action<T> action);
    }
}