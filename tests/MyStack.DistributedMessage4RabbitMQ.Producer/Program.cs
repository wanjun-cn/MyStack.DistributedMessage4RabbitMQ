using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyStack.DistributedMessage4RabbitMQ.Shared;

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
                       configure.HostName = "127.0.0.1";
                       configure.VirtualHost = "/";
                       configure.Port = 5672;
                       configure.UserName = "admin";
                       configure.Password = "admin";
                       configure.RoutingKeyPrefix = $"123.123.";
                       configure.ExchangeOptions.Name = "MultiwayLogistics";
                       configure.ExchangeOptions.ExchangeType = "topic";
                       configure.ExchangeOptions.Durable = true;
                       configure.QueueOptions.Name = "Identity";
                       configure.QueueOptions.Durable = true;
                   },


                   Assembly.GetExecutingAssembly());


               });

            var app = builder.Build();

            var messageBus = app.Services.GetRequiredService<IDistributedMessageBus>();

            //messageBus.PublishAsync(new WrappedData());
            // Publish a message and wait for a reply
            while (true)
            {
                var i = Console.ReadLine();
                if (i == "Q")
                    break;
                try
                {
                    var hello = new HelloMessage()
                    {
                        Message = "Hello World"
                    };
                    hello.Metadata.AddMessageHeader("key1", "key1");
                    messageBus.PublishAsync(hello);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }
        }
    }
}