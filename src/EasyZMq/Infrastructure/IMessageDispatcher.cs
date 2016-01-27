using System;

namespace EasyZMq.Infrastructure
{
    public interface IMessageDispatcher : IDisposable
    {
        Subscription Subscribe<T>();
        void Dispatch(dynamic message);
    }
}