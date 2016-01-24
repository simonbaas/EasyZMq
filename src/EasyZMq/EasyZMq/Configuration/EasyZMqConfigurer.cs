using System;
using EasyZMq.Logging;
using EasyZMq.Serialization;
using EasyZMq.Sockets;
using NetMQ;

namespace EasyZMq.Configuration
{
    public class EasyZMqConfigurer
    {
        private readonly EasyZMqConfiguration _configuration;
        
        public EasyZMqConfigurer(EasyZMqConfiguration configuration)
        {
            _configuration = configuration;

            SetDefaultLogger();
            SetDefaultSerializer();
        }

        public void Use(ISerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            _configuration.Serializer = serializer;
        }

        public void Use(IEasyZMqLoggerFactory easyZMqLoggerFactory)
        {
            if (easyZMqLoggerFactory == null) throw new ArgumentNullException(nameof(easyZMqLoggerFactory));

            _configuration.EasyZMqLoggerFactory = easyZMqLoggerFactory;
        }

        public IEasyZMqSubscriberSocket AsSubscriber(string topic)
        {
            var context = NetMQContext.Create();
            var socket = context.CreateSubscriberSocket();
            socket.Subscribe(topic);

            return new EasyZMqSubscriberSocket(_configuration, context, socket);
        }

        public IEasyZMqPublisherSocket AsPublisher()
        {
            var context = NetMQContext.Create();
            var socket = context.CreatePublisherSocket();

            return new EasyZMqPublisherSocket(_configuration, context, socket);
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