using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets.Subscriber
{
    public static class SubscriberSocketConfigurerExtension
    {
        public static ISubscriberSocket AsSubscriber(this EasyZMqConfigurer configurer, string topic)
        {
            var serializer = configurer.Configuration.Serializer;
            var addressBinder = configurer.Configuration.AddressBinder;
            var loggerFactory = configurer.Configuration.LoggerFactory;
            var context = NetMQContext.Create();
            var socket = context.CreateSubscriberSocket();
            socket.Subscribe(topic);

            return new SubscriberSocket(serializer, addressBinder, loggerFactory, context, socket);
        }
    }
}
