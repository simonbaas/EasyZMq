using System;
using EasyZMq.Logging;
using EasyZMq.Serialization;
using EasyZMq.Sockets;
using NetMQ;

namespace EasyZMq.Configuration
{
    public class EasyZMqConfigurer
    {
        public EasyZMqConfiguration Configuration { get; private set; }
        
        public EasyZMqConfigurer(EasyZMqConfiguration configuration)
        {
            Configuration = configuration;

            AddDefaultLogger();
            AddDefaultSerializer();
        }

        public EasyZMqConfigurer SetSerializer(ISerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");

            Configuration.Serializer = serializer;

            return this;
        }

        public EasyZMqConfigurer SetLogger(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");

            Configuration.Logger = logger;

            return this;
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

        private void AddDefaultSerializer()
        {
            Configuration.Serializer = new EasyZMqJsonSerializer();
        }

        private void AddDefaultLogger()
        {
            Configuration.Logger = new EasyZMqNullLogger();
        }
    }
}