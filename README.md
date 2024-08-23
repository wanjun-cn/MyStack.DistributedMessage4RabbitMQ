# MyStack.DistributedMessage4RabbitMQ

开源的轻量级消息总线类库（RabbitMQ），支持发布/订阅、RPC

| nuget      | stats |
| ----------- | ----------- | 
| [![nuget](https://img.shields.io/nuget/v/MyStack.DistributedMessage4RabbitMQ.svg?style=flat-square)](https://www.nuget.org/packages/MyStack.DistributedMessage4RabbitMQ)    | [![stats](https://img.shields.io/nuget/dt/MyStack.DistributedMessage4RabbitMQ.svg?style=flat-square)](https://www.nuget.org/stats/packages/MyStack.DistributedMessage4RabbitMQ?groupby=Version)         |

# 开始使用

## 添加服务支持
``` 
services.AddDistributedMessage4RabbitMQ(configure =>
{
    configure.HostName = "localhost";
    configure.VirtualHost = "/";
    configure.Port = 5672;
    configure.UserName = "admin";
    configure.Password = "admin";
    configure.QueueOptions.Name = "MyStack";
    configure.ExchangeOptions.Name = "MyStack";
    configure.ExchangeOptions.ExchangeType = "topic";
    configure.RPCTimeout = 2000;
},
Assembly.GetExecutingAssembly());
```

## 1、事件订阅
### 定义事件
```
[MessageName("HelloMessage")]
public class HelloMessage : IDistributedEvent
{
    public string Message { get; set; }
}

```

### 订阅事件
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
### 发布事件
```
await messageBus.PublishAsync(new HelloMessage() { Message = "Hello" });
```


## 2、事件体订阅
### 定义事件数据
``` 
public class WrappedData 
{
    public string Message { get; set; }
}

```

### 订阅事件
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
### 发布事件
```
await messageBus.PublishAsync(new WrappedData());
```


## 3、RPC请求
### 定义请求
```
public class Ping : IRpcRequest<Pong>
{
    public string SendBy { get; set; }
}
```
### 定义响应
```
 public class Pong
 {
     public string ReplyBy { get; set; }
 }
```

### 订阅消息
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
### 发布消息
```
var pongMessage = messageBus.SendAsync(ping);
```

# 许可证

MIT