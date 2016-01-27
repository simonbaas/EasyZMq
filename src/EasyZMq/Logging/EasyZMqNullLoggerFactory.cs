using System;

namespace EasyZMq.Logging
{
    public class EasyZMqNullEasyZMqLoggerFactory : AbstractEasyZMqLoggerFactory
    {
        public override ILogger GetLogger(Type type)
        {
            return new EasyZMqNullLogger();
        }
    }
}
