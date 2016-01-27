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
            var configuration = new EasyZMqConfiguration { AddressBinder = new ConnectAddressBinder(uri) };

            return new EasyZMqConfigurer(configuration);
        }

        public static EasyZMqConfigurer BindTo(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

            var uri = new Uri(address);
            var configuration = new EasyZMqConfiguration { AddressBinder = new BindAddressBinder(uri) };

            return new EasyZMqConfigurer(configuration);
        }

        public static EasyZMqConfigurer BindRandomPort(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

            var configuration = new EasyZMqConfiguration { AddressBinder = new BindRandomPortAddressBinder(address) };

            return new EasyZMqConfigurer(configuration);
        }
    }
}