using System;
using EasyZMq.Logging;

namespace EasyZMq.Log4net
{
    public class Log4NetLoggerFactory : AbstractLoggerFactory
    {
        public override ILogger GetLogger(Type type)
        {
            return new Log4NetLogger(type);
        }
    }
}
