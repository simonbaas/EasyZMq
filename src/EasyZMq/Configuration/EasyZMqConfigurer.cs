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

        public void Use(IMessageDispatcher messageDispatcher)
        {
            if (messageDispatcher == null) throw new ArgumentNullException(nameof(messageDispatcher));

            _ioC.Register(() => messageDispatcher);
        }
    }
}