using EasyZMq.Infrastructure;
using EasyZMq.Logging;
using EasyZMq.Serialization;

namespace EasyZMq.Configuration
{
    public class EasyZMqConfiguration
    {
        public IAddressBinder AddressBinder { get; set; }
        public ISerializer Serializer { get; set; }
        public ILogger Logger { get; set; }
    }
}
