using System;
using System.Threading;
using System.Threading.Tasks;
using EasyZMq.Configuration;
using EasyZMq.Sockets.Publisher;
using EasyZMq.Sockets.Subscriber;
using Xunit;

namespace EasyZMq.Tests
{
    public class PubSubTests : IClassFixture<TestsFixture>
    {
        private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);

        [Fact]
        public void One_publisher_one_subscriber_publish_message_with_empty_topic()
        {
            var tcs = new TaskCompletionSource<string>();

            var subscriberTopic = string.Empty;
            var message = "This is the message";

            using (var publisher = CreatePublisher())
            {
                var port = publisher.Uri.Port;
                using (var subscriber = CreateSubscriber(port, subscriberTopic))
                {
                    subscriber.On<string>(m => { tcs.SetResult(m); });
                    subscriber.Start();

                    // Let the subscriber connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    publisher.PublishMessage(message);

                    tcs.Task.Wait(WaitTimeout);
                    Assert.Equal(message, tcs.Task.Result);
                }
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
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

                    tcs.Task.Wait(WaitTimeout);
                    Assert.Equal(message, tcs.Task.Result);
                }
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void One_publisher_multiple_subscribers(string topic)
        {
            var message = "This is the message";
            var tcs1 = new TaskCompletionSource<string>();
            var tcs2 = new TaskCompletionSource<string>();

            using (var publisher = CreatePublisher())
            {
                var port = publisher.Uri.Port;
                using (var subscriber1 = CreateSubscriber(port, topic))
                using (var subscriber2 = CreateSubscriber(port, topic))
                {
                    subscriber1.On<string>(m => { tcs1.SetResult(m); });
                    subscriber2.On<string>(m => { tcs2.SetResult(m); });

                    subscriber1.Start();
                    subscriber2.Start();

                    // Let the subscribers connect to the publisher before publishing a message
                    Thread.Sleep(500);

                    publisher.PublishMessage(topic, message);

                    Task.WaitAll(new Task[] { tcs1.Task, tcs2.Task }, WaitTimeout);

                    Assert.Equal(message, tcs1.Task.Result);
                    Assert.Equal(message, tcs2.Task.Result);
                }
            }
        }

        [Fact]
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

                    Task.WaitAll(new Task[] { tcs1.Task, tcs2.Task }, WaitTimeout);

                    Assert.Equal(message1, tcs1.Task.Result);
                    Assert.Equal(message2, tcs2.Task.Result);
                }
            }
        }

        [Fact]
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

                    Task.WaitAll(new Task[] { tcs1.Task, tcs2.Task }, WaitTimeout);

                    var result1 = tcs1.Task.Result;
                    var result2 = tcs2.Task.Result;

                    Assert.IsAssignableFrom<TestMessage>(result1);
                    Assert.Equal(message1.MessageId, result1.MessageId);
                    Assert.Equal(message1.Payload, result1.Payload);

                    Assert.IsAssignableFrom<AnotherTestMessage>(result2);
                    Assert.Equal(message2.MessageId, result2.MessageId);
                    Assert.Equal(message2.Payload, result2.Payload);
                }
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void One_dynamic_publisher_one_dynamic_subscriber(string topic)
        {
            dynamic message = "This is the message's payload";

            var tcs = new TaskCompletionSource<dynamic>();

            using (var publisher = CreateDynamicPublisher())
            {
                var port = publisher.Uri.Port;

                using (var subscriber = CreateDynamicSubscriber(port, topic))
                {
                    subscriber.On(m => tcs.SetResult(m));
                    subscriber.Start();

                    // Let the subscriber connect to the publisher before publishing messages
                    Thread.Sleep(500);

                    publisher.PublishMessage(topic, message);

                    tcs.Task.Wait(WaitTimeout);

                    var result = tcs.Task.Result;

                    Assert.Equal(message, result);
                }
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void One_dynamic_publisher_one_dynamic_subscriber_rich_object_message(string topic)
        {
            var message = new TestMessage
            {
                MessageId = Guid.NewGuid(),
                Payload = "This is the message's payload"
            };

            var tcs = new TaskCompletionSource<dynamic>();

            using (var publisher = CreateDynamicPublisher())
            {
                var port = publisher.Uri.Port;

                using (var subscriber = CreateDynamicSubscriber(port, topic))
                {
                    subscriber.On(m => tcs.SetResult(m));
                    subscriber.Start();

                    // Let the subscriber connect to the publisher before publishing messages
                    Thread.Sleep(500);

                    publisher.PublishMessage(topic, message);

                    tcs.Task.Wait(WaitTimeout);

                    var result = tcs.Task.Result;

                    var receivedMessageId = (Guid)result.MessageId;
                    var receivedPayload = (string)result.Payload;

                    Assert.Equal(message.MessageId, receivedMessageId);
                    Assert.Equal(message.Payload, receivedPayload);
                }
            }
        }

        private static IPublisherSocket CreatePublisher()
        {
            return EasyZMqConfigure.BindRandomPort("tcp://localhost").AsPublisher();
        }

        private static IDynamicPublisherSocket CreateDynamicPublisher()
        {
            return EasyZMqConfigure.BindRandomPort("tcp://localhost").AsDynamicPublisher();
        }

        private static ISubscriberSocket CreateSubscriber(int port, string topic)
        {
            return EasyZMqConfigure.ConnectTo($"tcp://localhost:{port}").AsSubscriber(topic);
        }

        private static IDynamicSubscriberSocket CreateDynamicSubscriber(int port, string topic)
        {
            return EasyZMqConfigure.ConnectTo($"tcp://localhost:{port}").AsDynamicSubscriber(topic);
        }
    }
}
