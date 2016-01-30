using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets.Subscriber
{
    public static class SubscriberSocketConfigurerExtension
    {
        public static ISubscriberSocket AsSubscriber(this EasyZMqConfigurer configurer, string topic)
        {
            var context = NetMQContext.Create();
            var socket = context.CreateSubscriberSocket();
            socket.Subscribe(topic);

            return new SubscriberSocket(configurer.Configuration, context, socket);
        }
    }
}
