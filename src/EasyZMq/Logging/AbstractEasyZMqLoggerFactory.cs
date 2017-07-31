﻿using System;

namespace EasyZMq.Logging
{
    public abstract class AbstractLoggerFactory : ILoggerFactory
    {
        public abstract ILogger GetLogger(Type type);

        public ILogger GetLogger<T>()
        {
            return GetLogger(typeof (T));
        }
    }
}