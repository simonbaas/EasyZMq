using System;

namespace EasyZMq.Infrastructure
{
    internal class Subscription
    {
        public event Action<dynamic> Received;

        public void OnReceived(dynamic message)
        {
            var handler = Received;
            handler?.Invoke(message);
        }
    }
}