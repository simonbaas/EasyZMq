using System;
using EasyZMq.Infrastructure;

namespace EasyZMq.Configuration
{
    public static class EasyZMqConfigure
    {
        public static EasyZMqConfigurer ConnectTo(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

            var uri = new Uri(address);
            var addressBinder = new ConnectAddressBinder(uri);

            return CreateConfigurer(addressBinder);
        }

        public static EasyZMqConfigurer BindTo(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

            var uri = new Uri(address);
            var addressBinder = new BindAddressBinder(uri);

            return CreateConfigurer(addressBinder);
        }

        public static EasyZMqConfigurer BindRandomPort(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

            var addressBinder = new BindRandomPortAddressBinder(address);

            return CreateConfigurer(addressBinder);
        }

        private static EasyZMqConfigurer CreateConfigurer(IAddressBinder addressBinder)
        {
            var configurer = new EasyZMqConfigurer();
            configurer.Use(addressBinder);
            return configurer;
        }
    }
}