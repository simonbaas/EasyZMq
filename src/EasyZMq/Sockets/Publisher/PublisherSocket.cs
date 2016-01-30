using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets.Publisher
{
    public class PublisherSocket : AbstractSenderSocket, IPublisherSocket
    {
        public PublisherSocket(EasyZMqConfiguration configuration, NetMQContext context, NetMQSocket socket)
            : base(configuration, context, socket)
        { }

        public void PublishMessage<T>(T message)
        {
            SendMessage("", message);
        }

        public void PublishMessage<T>(string topic, T message)
        {
            SendMessage(topic, message);
        }
    }
}
