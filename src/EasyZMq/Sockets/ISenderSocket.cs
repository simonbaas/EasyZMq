namespace EasyZMq.Sockets
{
    public interface ISenderSocket : ISocket
    {
        void SendMessage<T>(string topic, T message);
    }
}