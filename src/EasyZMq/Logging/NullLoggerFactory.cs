using System;

namespace EasyZMq.Logging
{
    public class NullLoggerFactory : AbstractLoggerFactory
    {
        public override ILogger GetLogger(Type type)
        {
            return new NullLogger();
        }
    }
}
