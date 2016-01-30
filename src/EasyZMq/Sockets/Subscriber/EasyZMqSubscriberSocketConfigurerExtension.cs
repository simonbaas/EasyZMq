using EasyZMq.Configuration;
using NetMQ;

namespace EasyZMq.Sockets.Subscriber
{
    public static class EasyZMqSubscriberSocketConfigurerExtension
    {
        public static IEasyZMqSubscriberSocket AsSubscriber(this EasyZMqConfigurer configurer, string topic)
        {
            var context = NetMQContext.Create();
            var socket = context.CreateSubscriberSocket();
            socket.Subscribe(topic);

            return new EasyZMqSubscriberSocket(configurer.Configuration, context, socket);
        }
    }
}
