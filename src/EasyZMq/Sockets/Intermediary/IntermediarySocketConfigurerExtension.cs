using System;
using EasyZMq.Configuration;
using EasyZMq.Infrastructure;
using NetMQ;

namespace EasyZMq.Sockets.Intermediary
{
    public static class IntermediarySocketConfigurerExtension
    {
        public static IIntermediarySocket AsIntermediary(this EasyZMqConfigurer configurer, string backendAddress)
        {
            var frontendAddressBinder = configurer.AddressBinder;
            var backendAddressBinder = new BindAddressBinder(new Uri(backendAddress));

            return CreateIntermediarySocket(frontendAddressBinder, backendAddressBinder);
        }

        public static IIntermediarySocket AsIntermediaryWithRandomPort(this EasyZMqConfigurer configurer,
            string backendAddress)
        {
            var frontendAddressBinder = configurer.AddressBinder;
            var backendAddressBinder = new BindRandomPortAddressBinder(backendAddress);

            return CreateIntermediarySocket(frontendAddressBinder, backendAddressBinder);
        }

        private static IIntermediarySocket CreateIntermediarySocket(IAddressBinder frontendAddressBinder,
            IAddressBinder backendAddressBinder)
        {
            var context = NetMQContext.Create();
            var frontendSocket = context.CreateXSubscriberSocket();
            var backendSocket = context.CreateXPublisherSocket();

            return new IntermediarySocket(frontendAddressBinder, backendAddressBinder, frontendSocket, backendSocket);
        }
    }
}
