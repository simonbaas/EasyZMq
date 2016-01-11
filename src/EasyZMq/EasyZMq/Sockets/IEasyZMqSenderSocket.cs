namespace EasyZMq.Sockets
{
    public interface IEasyZMqSenderSocket : IEasyZMqSocket
    {
        void SendMessage<T>(string topic, T message);
    }
}