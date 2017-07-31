using System;

namespace EasyZMq.Infrastructure
{
    internal interface IMessageDispatcher : IDisposable
    {
        Subscription Subscribe<T>();
        void Dispatch(dynamic message);
    }
}