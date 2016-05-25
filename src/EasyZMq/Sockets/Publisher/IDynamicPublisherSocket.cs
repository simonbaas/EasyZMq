using System;

namespace EasyZMq.Sockets.Publisher
{
    public interface IDynamicPublisherSocket : IDisposable
    {
        Uri Uri { get; }
        void PublishMessage(string topic, dynamic message);
        void PublishMessage(dynamic message);
    }
}
