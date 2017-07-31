using EasyZMq.Configuration;

namespace EasyZMq.Log4Net
{
    public static class Log4NetConfigurerExtension
    {
        public static EasyZMqConfigurer UseLog4Net(this EasyZMqConfigurer configurer)
        {
            configurer.Use(new Log4NetLoggerFactory());

            return configurer;
        }
    }
}
