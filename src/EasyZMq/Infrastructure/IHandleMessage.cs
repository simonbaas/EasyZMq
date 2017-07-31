namespace EasyZMq.Infrastructure
{
    internal interface IHandleMessage<in T>
    {
        void Handle(T message);
    }
}