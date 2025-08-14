# DistributedMessage4RabbitMQ

An open-source lightweight message bus library (RabbitMQ) that supports publish/subscribe and RPC.

| nuget      | stats |
| ----------- | ----------- | 
| [![nuget](https://img.shields.io/nuget/v/DistributedMessage4RabbitMQ.svg?style=flat-square)](https://www.nuget.org/packages/DistributedMessage4RabbitMQ)    | [![stats](https://img.shields.io/nuget/dt/DistributedMessage4RabbitMQ.svg?style=flat-square)](https://www.nuget.org/stats/packages/DistributedMessage4RabbitMQ?groupby=Version)         |

# Install DistributedMessage4RabbitMQ

You can install via NuGet:
```csharp
Install-Package DistributedMessage4RabbitMQ

# Getting Started

## Add Service Support
```csharp 
services.AddDistributedMessage4RabbitMQ(configure =>
{
    configure.HostName = "localhost";
    configure.VirtualHost = "/";
    configure.Port = 5672;
    configure.UserName = "admin";
    configure.Password = "admin";
},
Assembly.GetExecutingAssembly());
```

## Event Subscription
### Define Event
```
public class HelloMessage : DistributedEventBase
{
    public string Message { get; set; }
}

or

[ExchangeDeclare("Hello")]
[QueueDeclare("Hello")]
[QueueBind("HelloMessage")]
public class HelloMessage : DistributedEventBase
{
    public string Message { get; set; } = default!;
}
```

### Subscribe to Event  
```
public class HelloMessageHandler : IDistributedEventHandler<HelloMessage>
{
    public async Task HandleAsync(HelloMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine("Hello");
        await Task.CompletedTask;
    }
}
```
### Publish Event
```
await messageBus.PublishAsync(new HelloMessage() { Message = "Hello" });

or 

var hello = new HelloMessage()
{
    Message = "Hello World"
};
hello.Metadata.AddMessageHeader("key1", "value");
messageBus.PublishAsync(hello);
```


## Event Wrapper Subscription
### Define Wrapped Data
``` 
public class WrappedData 
{
    public string Message { get; set; }
}

```

### Subscribe to Event
```
public class DistributedEventWrapperHandler : IDistributedEventHandler<DistributedEventWrapper<WrappedData>>
{
    public async Task HandleAsync(DistributedEventWrapper<WrappedData> eventData, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        Console.WriteLine("DistributedEventWrapper");
    }
}
```
### Publish Event
```
await messageBus.PublishAsync(new WrappedData());
```


## 3、RPC Request
### Define Request
```
public class Ping : IRpcRequest<Pong>
{
    public string SendBy { get; set; }
}
```
### Define Response
```
 public class Pong
 {
     public string ReplyBy { get; set; }
 }
```

### Subscribe to Request
```
  public class PingHandler : IRpcRequestHandler<Ping, Pong>
  {

      public Task<Pong> HandleAsync(Ping message, CancellationToken cancellationToken = default)
      {
          Console.WriteLine("Ping");
          return Task.FromResult(new Pong() { ReplyBy = "B" });
      }
  }
```
### Send Request
```
var pongMessage = messageBus.SendAsync(ping);
```

## 4、Dead Letter
### Define Message
```
[QueueBind("HelloMessage", QueueName = "Hello")]
[DeadLetter(messageType: typeof(HelloMessageDeadLetter))]
public class HelloMessage : DistributedEventBase
{
    public string Message { get; set; } = default!;
}
```

### Define Dead Letter Message
```
[QueueBind("HelloMessageDeadLetter", QueueName = "HelloDeadLetter", ExchangeName = "DeadLetter")]
public class HelloMessageDeadLetter : DistributedEventBase
{
}
```

# License

MIT