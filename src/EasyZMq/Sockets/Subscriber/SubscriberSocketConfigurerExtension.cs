using EasyZMq.Configuration;
using EasyZMq.Serialization;
using NetMQ;

namespace EasyZMq.Sockets.Subscriber
{
    public static class SubscriberSocketConfigurerExtension
    {
        public static ISubscriberSocket AsSubscriber(this EasyZMqConfigurer configurer, string topic)
        {
            var serializer = configurer.Serializer;
            var addressBinder = configurer.AddressBinder;
            var loggerFactory = configurer.LoggerFactory;
            var messageDispatcher = configurer.MessageDispatcher;

            var context = NetMQContext.Create();
            var socket = context.CreateSubscriberSocket();
            socket.Subscribe(topic);

            return new SubscriberSocket(serializer, addressBinder, loggerFactory, messageDispatcher, context, socket);
        }

        public static IDynamicSubscriberSocket AsDynamicSubscriber(this EasyZMqConfigurer configurer, string topic)
        {
            var serializer = new TypeUnawareJsonSerializer();
            var addressBinder = configurer.AddressBinder;
            var loggerFactory = configurer.LoggerFactory;
            var messageDispatcher = configurer.MessageDispatcher;

            var context = NetMQContext.Create();
            var socket = context.CreateSubscriberSocket();
            socket.Subscribe(topic);

            return new SubscriberSocket(serializer, addressBinder, loggerFactory, messageDispatcher, context, socket);
        }
    }
}
