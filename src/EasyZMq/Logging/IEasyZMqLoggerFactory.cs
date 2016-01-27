using System;

namespace EasyZMq.Logging
{
    public interface IEasyZMqLoggerFactory
    {
        ILogger GetLogger(Type type);
        ILogger GetLogger<T>();
    }
}
