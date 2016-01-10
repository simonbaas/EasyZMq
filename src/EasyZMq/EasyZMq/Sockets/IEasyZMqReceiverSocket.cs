namespace EasyZMq.Sockets
{
    public interface IEasyZMqReceiverSocket
    {
        void Start();
        void OnMessageReceived<T>(T message);
    }
}