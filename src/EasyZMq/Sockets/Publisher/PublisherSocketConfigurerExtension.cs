using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets.Publisher
{
    public static class PublisherSocketConfigurerExtension
    {
        public static IPublisherSocket AsPublisher(this EasyZMqConfigurer configurer)
        {
            var serializer = configurer.Serializer;
            var addressBinder = configurer.AddressBinder;
            var context = NetMQContext.Create();
            var socket = context.CreatePublisherSocket();

            return new PublisherSocket(serializer, addressBinder, context, socket);
        }
    }
}
