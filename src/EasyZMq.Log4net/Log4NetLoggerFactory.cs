using System;
using EasyZMq.Logging;

namespace EasyZMq.Log4Net
{
    public class Log4NetLoggerFactory : AbstractLoggerFactory
    {
        public override ILogger GetLogger(Type type)
        {
            return new Log4NetLogger(type);
        }
    }
}
