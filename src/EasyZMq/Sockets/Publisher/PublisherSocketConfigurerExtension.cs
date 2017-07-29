using EasyZMq.Configuration;
using EasyZMq.Serialization;
using NetMQ;

namespace EasyZMq.Sockets.Publisher
{
    public static class PublisherSocketConfigurerExtension
    {
        public static IPublisherSocket AsPublisher(this EasyZMqConfigurer configurer)
        {
            var serializer = configurer.Serializer;
            var addressBinder = configurer.AddressBinder;
            var socket = new NetMQ.Sockets.PublisherSocket();

            return new PublisherSocket(serializer, addressBinder, socket);
        }

        public static IDynamicPublisherSocket AsDynamicPublisher(this EasyZMqConfigurer configurer)
        {
            var serializer = new TypeUnawareJsonSerializer();
            var addressBinder = configurer.AddressBinder;
            var socket = new NetMQ.Sockets.PublisherSocket();

            return new PublisherSocket(serializer, addressBinder, socket);
        }
    }
}
