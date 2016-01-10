using System;

namespace EasyZMq.Sockets
{
    public interface IEasyZMqPublisherSocket : IDisposable
    {
        void PublishMessage<T>(string topic, T message);
        void PublishMessage<T>(T message);
    }
}