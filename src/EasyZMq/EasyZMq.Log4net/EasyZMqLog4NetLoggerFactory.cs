using System;
using EasyZMq.Logging;

namespace EasyZMq.Log4net
{
    public class EasyZMqLog4NetLoggerFactory : AbstractEasyZMqLoggerFactory
    {
        public override ILogger GetLogger(Type type)
        {
            return new EasyZMqLog4NetLogger(type);
        }
    }
}
