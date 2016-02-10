namespace EasyZMq.Sockets
{
    internal interface IReceiverSocket : IStartableSocket, ISocket
    {
        void OnMessageReceived<T>(T message);
    }
}