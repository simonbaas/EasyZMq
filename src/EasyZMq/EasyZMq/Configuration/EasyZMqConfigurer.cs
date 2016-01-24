using System;
using EasyZMq.Logging;
using EasyZMq.Serialization;

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
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            Configuration.Serializer = serializer;
        }

        public void Use(IEasyZMqLoggerFactory easyZMqLoggerFactory)
        {
            if (easyZMqLoggerFactory == null) throw new ArgumentNullException(nameof(easyZMqLoggerFactory));

            Configuration.EasyZMqLoggerFactory = easyZMqLoggerFactory;
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