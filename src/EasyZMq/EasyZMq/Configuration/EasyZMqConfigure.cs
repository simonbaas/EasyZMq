using System;
using EasyZMq.Infrastructure;

namespace EasyZMq.Configuration
{
    public static class EasyZMqConfigure
    {
        public static EasyZMqConfigurer ConnectTo(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException("address");

            var configuration = new EasyZMqConfiguration();
            configuration.AddressBinder = new ConnectAddressBinder(address);

            return new EasyZMqConfigurer(configuration);
        }

        public static EasyZMqConfigurer BindTo(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException("address");

            var configuration = new EasyZMqConfiguration();
            configuration.AddressBinder = new BindAddressBinder(address);

            return new EasyZMqConfigurer(configuration);
        }
    }
}