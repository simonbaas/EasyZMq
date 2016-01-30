using System;
using EasyZMq.Infrastructure;
using EasyZMq.Logging;
using EasyZMq.Serialization;

namespace EasyZMq.Configuration
{
    public class EasyZMqConfigurer
    {
        private readonly SuperSimpleIoC _ioC;

        public EasyZMqConfigurer()
        {
            _ioC = new SuperSimpleIoC();

            RegisterDefaultLogger();
            RegisterDefaultSerializer();
            RegisterDefaultMessageDispatcher();
        }

        public IAddressBinder AddressBinder => _ioC.Resolve<IAddressBinder>();
        public ISerializer Serializer => _ioC.Resolve<ISerializer>();
        public ILoggerFactory LoggerFactory => _ioC.Resolve<ILoggerFactory>();
        public IMessageDispatcher MessageDispatcher => _ioC.Resolve<IMessageDispatcher>();

        public void Use(ISerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            _ioC.Register(() => serializer);
        }

        public void Use(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            _ioC.Register(() => loggerFactory);
        }

        public void Use(IAddressBinder addressBinder)
        {
            if (addressBinder == null) throw new ArgumentNullException(nameof(addressBinder));

            _ioC.Register(() => addressBinder);
        }

        private void RegisterDefaultSerializer()
        {
            Use(new JsonSerializer());
        }

        private void RegisterDefaultLogger()
        {
            Use(new NullLoggerFactory());
        }

        private void RegisterDefaultMessageDispatcher()
        {
            _ioC.Register<IMessageDispatcher, MessageDispatcher>();
        }
    }
}