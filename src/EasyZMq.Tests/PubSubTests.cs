using System;
using System.Threading;
using System.Threading.Tasks;
using EasyZMq.Configuration;
using EasyZMq.Sockets.Publisher;
using EasyZMq.Sockets.Subscriber;
using NUnit.Framework;

namespace EasyZMq.Tests
{
    [TestFixture(Author = "Simon Baas", Description = "Publish/subscribe tests")]
    public class PubSubTests
    {
        [TestCase("", Description = "Empty topic")]
        [TestCase("A", Description = "Non-empty topic")]
        public void One_publisher_one_subscriber(string topic)
        {
            var message = "This is the message";
            var tcs = new TaskCompletionSource<string>();

            using (var publisher = CreatePublisher())
            {
                var port = publisher.Uri.Port;
                using (var subscriber = CreateSubscriber(port, topic))
                {
                    subscriber.On<string>(m => { tcs.SetResult(m); });
                    subscriber.Start();

                    // Let the subscriber connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    publisher.PublishMessage(topic, message);

                    tcs.Task.Wait();
                    Assert.AreEqual(message, tcs.Task.Result);
                }
            }
        }        

        [TestCase("", Description = "Empty topic")]
        [TestCase("A", Description = "Non-empty topic")]
        public void One_publisher_multiple_subscribers(string topic)
        {
            var message = "This is the message";
            var tcs1 = new TaskCompletionSource<string>();
            var tcs2 = new TaskCompletionSource<string>();

            using (var publisher = CreatePublisher())
            {
                var port = publisher.Uri.Port;
                using (var subscriber1 = CreateSubscriber(port, topic))
                {
                    using (var subscriber2 = CreateSubscriber(port, topic))
                    {
                        subscriber1.On<string>(m => { tcs1.SetResult(m); });
                        subscriber2.On<string>(m => { tcs2.SetResult(m); });

                        subscriber1.Start();
                        subscriber2.Start();

                        // Let the subscribers connect to the publisher before publishing a message
                        Thread.Sleep(500);

                        publisher.PublishMessage(topic, message);

                        Task.WaitAll(tcs1.Task, tcs2.Task);

                        Assert.AreEqual(message, tcs1.Task.Result);
                        Assert.AreEqual(message, tcs2.Task.Result);
                    }
                }
            }
        }

        [Test]
        public void One_publisher_multiple_subscribers_different_topics()
        {
            var message1 = "This is the message - Topic A";
            var message2 = "This is the message - Topic B";
            var topic1 = "A";
            var topic2 = "B";

            var tcs1 = new TaskCompletionSource<string>();
            var tcs2 = new TaskCompletionSource<string>();

            using (var publisher = CreatePublisher())
            {
                var port = publisher.Uri.Port;
                using (var subscriber1 = CreateSubscriber(port, topic1))
                {
                    using (var subscriber2 = CreateSubscriber(port, topic2))
                    {
                        subscriber1.On<string>(m => { tcs1.SetResult(m); });
                        subscriber2.On<string>(m => { tcs2.SetResult(m); });

                        subscriber1.Start();
                        subscriber2.Start();

                        // Let the subscribers connect to the publisher before publishing messages
                        Thread.Sleep(500);

                        publisher.PublishMessage(topic1, message1);
                        publisher.PublishMessage(topic2, message2);

                        Task.WaitAll(tcs1.Task, tcs2.Task);

                        Assert.AreEqual(message1, tcs1.Task.Result);
                        Assert.AreEqual(message2, tcs2.Task.Result);
                    }
                }
            }
        }

        [Test]
        public void One_publisher_one_subscriber_rich_object_messages()
        {
            var topic = "";

            var message1 = new TestMessage
            {
                MessageId = Guid.NewGuid(),
                Payload = "This is the message's payload"
            };

            var message2 = new AnotherTestMessage
            {
                MessageId = Guid.NewGuid(),
                Payload = "This is another message's payload"
            };

            var tcs1 = new TaskCompletionSource<TestMessage>();
            var tcs2 = new TaskCompletionSource<AnotherTestMessage>();

            using (var publisher = CreatePublisher())
            {
                var port = publisher.Uri.Port;
                using (var subscriber = CreateSubscriber(port, topic))
                {
                    subscriber.On<TestMessage>(m => { tcs1.SetResult(m); });
                    subscriber.On<AnotherTestMessage>(m => { tcs2.SetResult(m); });
                    subscriber.Start();

                    // Let the subscriber connect to the publisher before publishing messages
                    Thread.Sleep(500);

                    publisher.PublishMessage(topic, message1);
                    publisher.PublishMessage(topic, message2);

                    Task.WaitAll(tcs1.Task, tcs2.Task);

                    var result1 = tcs1.Task.Result;
                    var result2 = tcs2.Task.Result;

                    Assert.IsAssignableFrom<TestMessage>(result1);
                    Assert.AreEqual(message1.MessageId, result1.MessageId);
                    Assert.AreEqual(message1.Payload, result1.Payload);

                    Assert.IsAssignableFrom<AnotherTestMessage>(result2);
                    Assert.AreEqual(message2.MessageId, result2.MessageId);
                    Assert.AreEqual(message2.Payload, result2.Payload);
                }
            }
        }

        private static IPublisherSocket CreatePublisher()
        {
            return EasyZMqConfigure.BindRandomPort("tcp://localhost").AsPublisher();
        }

        private static ISubscriberSocket CreateSubscriber(int port, string topic)
        {
            return EasyZMqConfigure.ConnectTo($"tcp://localhost:{port}").AsSubscriber(topic);
        }

        private class TestMessage
        {
            public Guid MessageId { get; set; }
            public string Payload { get; set; }
        }

        private class AnotherTestMessage : TestMessage
        {
            
        }
    }
}
