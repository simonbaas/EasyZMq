namespace EasyZMq.Sockets
{
    public interface IEasyZMqReceiverSocket : IEasyZMqSocket
    {
        void Start();
        void OnMessageReceived<T>(T message);
    }
}