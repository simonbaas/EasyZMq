using EasyZMq.Configuration;

namespace EasyZMq.Log4net
{
    public static class Log4NetConfigurerExtension
    {
        public static EasyZMqConfigurer UseLog4Net(this EasyZMqConfigurer configurer)
        {
            configurer.Use(new EasyZMqLog4NetLoggerFactory());

            return configurer;
        }
    }
}
