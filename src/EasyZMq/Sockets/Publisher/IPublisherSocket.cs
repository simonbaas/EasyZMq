namespace EasyZMq.Sockets.Publisher
{
    public interface IPublisherSocket : ISocket
    {
        void PublishMessage<T>(string topic, T message);
        void PublishMessage<T>(T message);
    }
}