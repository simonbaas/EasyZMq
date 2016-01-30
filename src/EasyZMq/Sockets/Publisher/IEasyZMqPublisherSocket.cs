using System;

namespace EasyZMq.Sockets.Publisher
{
    public interface IEasyZMqPublisherSocket : IDisposable
    {
        Uri Uri { get; }
        void PublishMessage<T>(string topic, T message);
        void PublishMessage<T>(T message);
    }
}