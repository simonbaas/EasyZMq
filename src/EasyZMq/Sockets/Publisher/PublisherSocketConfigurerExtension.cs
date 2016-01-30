using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets.Publisher
{
    public static class PublisherSocketConfigurerExtension
    {
        public static IPublisherSocket AsPublisher(this EasyZMqConfigurer configurer)
        {
            var context = NetMQContext.Create();
            var socket = context.CreatePublisherSocket();

            return new PublisherSocket(configurer.Configuration, context, socket);
        }
    }
}
