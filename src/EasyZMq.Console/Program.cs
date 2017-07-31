using System;
using NetMQ;
using NetMQ.Monitoring;
using NetMQ.Sockets;

namespace EasyZMq.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var publisher = new PublisherSocket())
            {
                publisher.Bind("tcp://localhost:1234");

                using (var subscriber = new SubscriberSocket())
                {
                    subscriber.Subscribe("A");

                    using (var poller = new NetMQPoller { subscriber })
                    {
                        using (var monitor = new NetMQMonitor(subscriber, "inproc://sub.inproc", SocketEvents.Connected | SocketEvents.Disconnected | SocketEvents.ConnectRetried))
                        {
                            monitor.AttachToPoller(poller);

                            subscriber.Connect("tcp://localhost:1234");

                            poller.RunAsync();

                            System.Console.ReadKey();
                        }
                    }
                }
            }
        }
    }
}