using System;
using EasyZMq.Logging;
using EasyZMq.Serialization;
using EasyZMq.Sockets;
using NetMQ;

namespace EasyZMq.Configuration
{
    public class EasyZMqConfigurer
    {
        public EasyZMqConfiguration Configuration { get; }
        
        public EasyZMqConfigurer(EasyZMqConfiguration configuration)
        {
            Configuration = configuration;

            SetDefaultLogger();
            SetDefaultSerializer();
        }

        public void Use(ISerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");

            Configuration.Serializer = serializer;
        }

        public void Use(IEasyZMqLoggerFactory easyZMqLoggerFactory)
        {
            if (easyZMqLoggerFactory == null) throw new ArgumentNullException("easyZMqLoggerFactory");

            Configuration.EasyZMqLoggerFactory = easyZMqLoggerFactory;
        }

        public IEasyZMqSubscriberSocket AsSubscriber(string topic)
        {
            var context = NetMQContext.Create();
            var socket = context.CreateSubscriberSocket();
            socket.Subscribe(topic);

            return new EasyZMqSubscriberSocket(Configuration, context, socket);
        }

        public IEasyZMqPublisherSocket AsPublisher()
        {
            var context = NetMQContext.Create();
            var socket = context.CreatePublisherSocket();

            return new EasyZMqPublisherSocket(Configuration, context, socket);
        }

        private void SetDefaultSerializer()
        {
            Use(new EasyZMqJsonSerializer());
        }

        private void SetDefaultLogger()
        {
            Use(new EasyZMqNullEasyZMqLoggerFactory());
        }
    }
}