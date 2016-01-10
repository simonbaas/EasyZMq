using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyZMq.Infrastructure
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly Dictionary<Type, Subscription> _subscriptions = new Dictionary<Type, Subscription>();
        private readonly BlockingCollection<dynamic> _queue = new BlockingCollection<dynamic>();
        private Task _task;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public MessageDispatcher()
        {
            _task = StartDispatcher();
        }

        public Subscription Subscribe<T>()
        {
            var type = typeof(T);
            Subscription subscription;
            if (!_subscriptions.TryGetValue(type, out subscription))
            {
                subscription = new Subscription();
                _subscriptions.Add(type, subscription);
            }

            return subscription;
        }

        public void Dispatch(dynamic message)
        {
            _queue.Add(message);
        }
        
        private Task StartDispatcher()
        {
            return Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var message = _queue.Take(_cts.Token);
                        DispatchToSubscriber(message);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }
        
        private void DispatchToSubscriber(dynamic message)
        {
            try
            {
                Subscription subscription;
                if (_subscriptions.TryGetValue(message.GetType(), out subscription))
                {
                    subscription.OnReceived(message);
                }
            }
            catch (Exception ex)
            {
                // TODO: Log
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_task != null)
                    {
                        _cts.Cancel();
                        _task.Wait();

                        _cts = null;
                        _task = null;
                    }
                }

                _disposedValue = true;
            }
        }
    }
}