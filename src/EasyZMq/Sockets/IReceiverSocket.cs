namespace EasyZMq.Sockets
{
    internal interface IReceiverSocket : ISocket
    {
        void Start();
        void OnMessageReceived<T>(T message);
    }
}