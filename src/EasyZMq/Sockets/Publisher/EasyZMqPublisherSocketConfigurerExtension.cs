using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets.Publisher
{
    public static class EasyZMqPublisherSocketConfigurerExtension
    {
        public static IEasyZMqPublisherSocket AsPublisher(this EasyZMqConfigurer configurer)
        {
            var context = NetMQContext.Create();
            var socket = context.CreatePublisherSocket();

            return new EasyZMqPublisherSocket(configurer.Configuration, context, socket);
        }
    }
}
