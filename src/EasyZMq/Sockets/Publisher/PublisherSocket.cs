using EasyZMq.Infrastructure;
using EasyZMq.Serialization;
using NetMQ;

namespace EasyZMq.Sockets.Publisher
{
    public class PublisherSocket : AbstractSenderSocket, IPublisherSocket
    {
        public PublisherSocket(ISerializer serializer, IAddressBinder addressBinder, NetMQContext context, NetMQSocket socket)
            : base(serializer, addressBinder, context, socket)
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
