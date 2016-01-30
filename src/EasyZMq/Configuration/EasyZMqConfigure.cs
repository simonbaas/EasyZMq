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

            return new EasyZMqConfigurer(addressBinder);
        }

        public static EasyZMqConfigurer BindTo(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

            var uri = new Uri(address);
            var addressBinder = new BindAddressBinder(uri);

            return new EasyZMqConfigurer(addressBinder);
        }

        public static EasyZMqConfigurer BindRandomPort(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

            var addressBinder = new BindRandomPortAddressBinder(address);

            return new EasyZMqConfigurer(addressBinder);
        }
    }
}