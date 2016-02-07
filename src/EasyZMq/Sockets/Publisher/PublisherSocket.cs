using EasyZMq.Infrastructure;
using EasyZMq.Serialization;
using NetMQ;

namespace EasyZMq.Sockets.Publisher
{
    internal class PublisherSocket : AbstractSenderSocket, IPublisherSocket
    {
        public PublisherSocket(ISerializer serializer, IAddressBinder addressBinder, NetMQSocket socket)
            : base(serializer, addressBinder, socket)
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
