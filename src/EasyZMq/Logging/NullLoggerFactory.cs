using System;

namespace EasyZMq.Logging
{
    internal class NullLoggerFactory : AbstractLoggerFactory
    {
        public override ILogger GetLogger(Type type)
        {
            return new NullLogger();
        }
    }
}
