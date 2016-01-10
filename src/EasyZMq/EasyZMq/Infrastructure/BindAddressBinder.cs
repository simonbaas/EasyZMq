using NetMQ;

namespace EasyZMq.Infrastructure
{
    public class BindAddressBinder : IAddressBinder
    {
        private readonly string _address;

        public BindAddressBinder(string address)
        {
            _address = address;
        }

        public void ConnectOrBindAddress(NetMQSocket socket)
        {
            socket.Bind(_address);
        }
    }
}
