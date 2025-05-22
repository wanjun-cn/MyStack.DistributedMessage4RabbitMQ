using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyStack.DistributedMessage4RabbitMQ.Shared;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
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
                       configure.HostName = "192.168.2.186";
                       configure.VirtualHost = "/";
                       configure.Port = 5672;
                       configure.UserName = "admin";
                       configure.Password = "admin";
                       configure.PrefetchCount = 100;
                       configure.RoutingKeyPrefix = $"*.*.";
                       configure.ExchangeOptions.Name = "MultiwayLogistics";
                       configure.ExchangeOptions.ExchangeType = "topic";
                       configure.ExchangeOptions.Durable = true;
                       configure.QueueOptions.Durable = true;
                   },
                   Assembly.GetExecutingAssembly());
                   services.Replace(new ServiceDescriptor(typeof(IMessageSerializer), typeof(CustomJsonMessageSerializer), ServiceLifetime.Transient));
               });

            var app = builder.Build();
            app.Run();
        }
    }
}