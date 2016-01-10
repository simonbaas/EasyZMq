namespace EasyZMq.Infrastructure
{
    public interface IHandleMessage<in T>
    {
        void Handle(T message);
    }
}