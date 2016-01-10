namespace EasyZMq.Sockets
{
    public interface IEasyZMqSenderSocket
    {
        void SendMessage<T>(string topic, T message);
    }
}