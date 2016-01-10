using NetMQ;

namespace EasyZMq.Infrastructure
{
    public interface IAddressBinder
    {
        void ConnectOrBindAddress(NetMQSocket socket);
    }
}
