using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EasyZMq.Logging;

namespace EasyZMq.Infrastructure
{
    internal class MessageDispatcher : IMessageDispatcher
    {
        private readonly ConcurrentDictionary<Type, Subscription> _subscriptions = new ConcurrentDictionary<Type, Subscription>();
        private readonly BlockingCollection<dynamic> _queue = new BlockingCollection<dynamic>();
        private readonly ILogger _logger;

        private Task _task;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public MessageDispatcher(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.GetLogger(typeof(MessageDispatcher));
            _task = StartDispatcher();
        }

        public Subscription Subscribe<T>()
        {
            var type = typeof(T);
            var subscription = _subscriptions.GetOrAdd(type, _ => new Subscription());
            return subscription;
        }

        public void Dispatch(dynamic message)
        {
            _queue.Add(message);
        }

        private Task StartDispatcher()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var options = new ParallelOptions { MaxDegreeOfParallelism = 1, CancellationToken = _cts.Token };
                    var partioner = Partitioner.Create(_queue.GetConsumingEnumerable(_cts.Token), EnumerablePartitionerOptions.NoBuffering);

                    Parallel.ForEach(partioner, options, message => DispatchToSubscriber(message));
                }
                catch (OperationCanceledException) { }
            }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void DispatchToSubscriber(dynamic message)
        {
            if (TryDynamicDispatch(message))
            {
                return;
            }

            Subscription subscription;
            if (!_subscriptions.TryGetValue(message.GetType(), out subscription))
            {
                _logger.Warning("No suitable subscriber found for message type: {0}", message.GetType().ToString());
                return;
            }

            try
            {
                subscription.OnReceived(message);
            }
            catch (Exception ex)
            {
                _logger.Error($"Subscriber for message type {message.GetType()} threw an unhandled exception", ex);
            }
        }

        private bool TryDynamicDispatch(dynamic message)
        {
            Subscription subscription;
            if (!_subscriptions.TryGetValue(typeof(object), out subscription))
            {
                return false;
            }

            try
            {
                subscription.OnReceived(message);
            }
            catch (Exception ex)
            {
                _logger.Error("Subscriber for dynamically dispatched message threw an unhandled exception", ex);
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

                        _cts.Dispose();

                        _cts = null;
                        _task = null;
                    }
                }

                _disposedValue = true;
            }
        }
    }
}