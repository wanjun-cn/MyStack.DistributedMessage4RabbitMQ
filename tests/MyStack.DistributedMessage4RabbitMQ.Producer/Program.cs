using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyStack.DistributedMessage4RabbitMQ.Shared;
using System.Reflection;

namespace MyStack.DistributedMessage4RabbitMQ.Producer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
               .ConfigureHostConfiguration(configure =>
               {
                   configure.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");
               })
               .ConfigureServices((context, services) =>
               {
                   services.AddLogging(logging =>
                   {
                       logging.AddConsole();
                   });
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
               });

            var app = builder.Build();

            var messageBus = app.Services.GetRequiredService<IDistributedMessageBus>();
            messageBus.PublishAsync(new HelloMessage()
            {
                Message = "Hello World"
            });
            messageBus.PublishAsync(new WrappedData());
            messageBus.PublishAsync("ABC",new SubscribeData());
            // Publish a message and wait for a reply
            while (true)
            {
                var i = Console.ReadLine();
                if (i == "Q")
                    break;
                var ping = new Ping()
                {
                    SendBy = "A"
                };
                var pongMessage = messageBus.SendAsync(ping).GetAwaiter().GetResult();
                Console.WriteLine(pongMessage?.ReplyBy);
            }
        }
    }
}