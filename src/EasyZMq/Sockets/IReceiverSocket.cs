namespace EasyZMq.Sockets
{
    public interface IReceiverSocket : ISocket
    {
        void Start();
        void OnMessageReceived<T>(T message);
    }
}