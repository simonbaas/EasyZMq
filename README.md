# EasyZMq
EasyZMq is a wrapper around NetMQ and its goal is to provide an easy api when working with ZeroMQ on .NET.

## Using
Publish/subscribe is currently supported by the library.

### Publish/subscribe
Publisher
```csharp
var topic = string.Empty;
using (var publisher = EasyZMqConfigure
    .BindTo("tcp://localhost:1337")
    .AsPublisher())
  {
      publisher.PublishMessage(topic, "This is the message");
  }
```
Subscriber
```csharp
var topic = string.Empty;
using (var subscriber = EasyZMqConfigure
    .ConnectTo("tcp://localhost:1337")
    .AsSubscriber(topic))
  {
      subscriber.On<string>(message => { Console.WriteLine("Message received: {0}", message); });
  
      subscriber.Start();
  
      Console.WriteLine("Waiting for messages");
      Console.ReadKey();
  }
```
## Requirements
.NET Framework 4.5
