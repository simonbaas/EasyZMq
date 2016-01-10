using NetMQ;

namespace EasyZMq.Infrastructure
{
    public class ConnectAddressBinder : IAddressBinder
    {
        private readonly string _address;

        public ConnectAddressBinder(string address)
        {
            _address = address;
        }

        public void ConnectOrBindAddress(NetMQSocket socket)
        {
            socket.Connect(_address);
        }
    }
}
