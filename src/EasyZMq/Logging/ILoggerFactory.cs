using System;

namespace EasyZMq.Logging
{
    public interface ILoggerFactory
    {
        ILogger GetLogger(Type type);
        ILogger GetLogger<T>();
    }
}
