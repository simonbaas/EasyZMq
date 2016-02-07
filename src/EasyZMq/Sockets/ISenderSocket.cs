namespace EasyZMq.Sockets
{
    internal interface ISenderSocket : ISocket
    {
        void SendMessage<T>(string topic, T message);
    }
}