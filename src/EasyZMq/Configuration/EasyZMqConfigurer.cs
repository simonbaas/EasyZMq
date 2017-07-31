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

            RegisterDefaultSerializer();
            RegisterDefaultLoggerFactory();
            RegisterDefaultMessageDispatcher();
        }

        internal ISerializer Serializer => _ioC.Resolve<ISerializer>();
        internal ILoggerFactory LoggerFactory => _ioC.Resolve<ILoggerFactory>();
        internal IAddressBinder AddressBinder => _ioC.Resolve<IAddressBinder>();
        internal IMessageDispatcher MessageDispatcher => _ioC.Resolve<IMessageDispatcher>();

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

        internal void Use(IAddressBinder addressBinder)
        {
            if (addressBinder == null) throw new ArgumentNullException(nameof(addressBinder));

            _ioC.Register(() => addressBinder);
        }

        private void RegisterDefaultSerializer()
        {
            _ioC.Register<ISerializer, TypeAwareJsonSerializer>();
        }

        private void RegisterDefaultLoggerFactory()
        {
            _ioC.Register<ILoggerFactory, NullLoggerFactory>();
        }

        private void RegisterDefaultMessageDispatcher()
        {
            _ioC.Register<IMessageDispatcher, MessageDispatcher>();
        }
    }
}