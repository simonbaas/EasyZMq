using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyZMq.Configuration;
using EasyZMq.Sockets.Intermediary;
using EasyZMq.Sockets.Publisher;
using EasyZMq.Sockets.Subscriber;
using Xunit;

namespace EasyZMq.Tests
{
    public class XPubSubTests : IClassFixture<TestsFixture>
    {
        private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void One_publisher_one_subscriber(string topic)
        {
            var message = "This is the message";

            var tcs = new TaskCompletionSource<string>();

            using (var intermediary = CreateIntermediary())
            {
                var frontendPort = intermediary.FrontendUri.Port;
                var backendPort = intermediary.BackendUri.Port;

                intermediary.Start();

                using (var publisher = CreatePublisher(frontendPort))
                using (var subscriber = CreateSubscriber(backendPort, topic))
                {
                    subscriber.On<string>(m => tcs.SetResult(m));

                    subscriber.Start();

                    // Let the subscribers connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    publisher.PublishMessage(topic, message);

                    tcs.Task.Wait(WaitTimeout);

                    Assert.Equal(message, tcs.Task.Result);
                }
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void Multiple_publishers_one_subscriber(string topic)
        {
            var message1 = "This is the message - 1";
            var message2 = "This is the message - 2";

            var receivedMessages = new List<string>();

            var tcs = new TaskCompletionSource<List<string>>();

            using (var intermediary = CreateIntermediary())
            {
                var frontendPort = intermediary.FrontendUri.Port;
                var backendPort = intermediary.BackendUri.Port;

                intermediary.Start();

                using (var publisher1 = CreatePublisher(frontendPort))
                using (var publisher2 = CreatePublisher(frontendPort))
                using (var subscriber = CreateSubscriber(backendPort, topic))
                {
                    subscriber.On<string>(m => { receivedMessages.Add(m); if (receivedMessages.Count >= 2) tcs.SetResult(receivedMessages); });

                    subscriber.Start();

                    // Let the subscribers connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    publisher1.PublishMessage(topic, message1);
                    publisher2.PublishMessage(topic, message2);

                    tcs.Task.Wait(WaitTimeout);

                    Assert.Contains(message1, receivedMessages);
                    Assert.Contains(message2, receivedMessages);
                }
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void Multiple_publishers_multiple_subscribers(string topic)
        {
            var message1 = "This is the message - 1";
            var message2 = "This is the message - 2";

            var receivedMessages1 = new List<string>();
            var receivedMessages2 = new List<string>();

            var tcs1 = new TaskCompletionSource<List<string>>();
            var tcs2 = new TaskCompletionSource<List<string>>();

            using (var intermediary = CreateIntermediary())
            {
                var frontendPort = intermediary.FrontendUri.Port;
                var backendPort = intermediary.BackendUri.Port;

                intermediary.Start();

                using (var publisher1 = CreatePublisher(frontendPort))
                using (var publisher2 = CreatePublisher(frontendPort))
                using (var subscriber1 = CreateSubscriber(backendPort, topic))
                using (var subscriber2 = CreateSubscriber(backendPort, topic))
                {
                    subscriber1.On<string>(m => { receivedMessages1.Add(m); if (receivedMessages1.Count >= 2) tcs1.SetResult(receivedMessages1); });
                    subscriber2.On<string>(m => { receivedMessages2.Add(m); if (receivedMessages2.Count >= 2) tcs2.SetResult(receivedMessages2); });

                    subscriber1.Start();
                    // Let the subscriber connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    subscriber2.Start();
                    // Let the subscriber connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    publisher1.PublishMessage(topic, message1);
                    publisher2.PublishMessage(topic, message2);

                    Task.WaitAll(new Task[] { tcs1.Task, tcs2.Task}, WaitTimeout);

                    Assert.Contains(message1, receivedMessages1);
                    Assert.Contains(message2, receivedMessages1);

                    Assert.Contains(message1, receivedMessages2);
                    Assert.Contains(message2, receivedMessages2);
                }
            }
        }

        [Fact]
        public void Multiple_publishers_multiple_subscribers_different_topics()
        {
            var topic1 = "Topic 1";
            var topic2 = "Topic 2";
            var message1 = "This is the message - 1";
            var message2 = "This is the message - 2";

            var receivedMessages1 = new List<string>();
            var receivedMessages2 = new List<string>();

            var tcs1 = new TaskCompletionSource<List<string>>();
            var tcs2 = new TaskCompletionSource<List<string>>();

            using (var intermediary = CreateIntermediary())
            {
                var frontendPort = intermediary.FrontendUri.Port;
                var backendPort = intermediary.BackendUri.Port;

                intermediary.Start();

                using (var publisher1 = CreatePublisher(frontendPort))
                using (var publisher2 = CreatePublisher(frontendPort))
                using (var subscriber1 = CreateSubscriber(backendPort, topic1))
                using (var subscriber2 = CreateSubscriber(backendPort, topic2))
                {
                    subscriber1.On<string>(m => { receivedMessages1.Add(m); if (receivedMessages1.Count >= 1) tcs1.SetResult(receivedMessages1); });
                    subscriber2.On<string>(m => { receivedMessages2.Add(m); if (receivedMessages2.Count >= 1) tcs2.SetResult(receivedMessages2); });

                    subscriber1.Start();
                    // Let the subscriber connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    subscriber2.Start();
                    // Let the subscriber connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    publisher1.PublishMessage(topic1, message1);
                    publisher2.PublishMessage(topic2, message2);

                    Task.WaitAll(new Task[] { tcs1.Task, tcs2.Task }, WaitTimeout);

                    Assert.Equal(1, receivedMessages1.Count);
                    Assert.Contains(message1, receivedMessages1);
                    
                    Assert.Equal(1, receivedMessages2.Count);
                    Assert.Contains(message2, receivedMessages2);
                }
            }
        }

        private static IIntermediarySocket CreateIntermediary()
        {
            return EasyZMqConfigure.BindRandomPort("tcp://localhost").AsIntermediaryWithRandomPort("tcp://localhost");
        }

        private static IPublisherSocket CreatePublisher(int port)
        {
            return EasyZMqConfigure.ConnectTo($"tcp://localhost:{port}").AsPublisher();
        }

        private static ISubscriberSocket CreateSubscriber(int port, string topic)
        {
            return EasyZMqConfigure.ConnectTo($"tcp://localhost:{port}").AsSubscriber(topic);
        }
    }
}
