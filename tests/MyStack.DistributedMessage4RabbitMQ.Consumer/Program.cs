﻿using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyStack.DistributedMessage4RabbitMQ.Producer;

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
                       configure.HostName = "localhost";
                       configure.VirtualHost = "/";
                       configure.Port = 5672;
                       configure.UserName = "admin";
                       configure.Password = "admin";
                       configure.QueueOptions.Name = "MyStack";
                       configure.ExchangeOptions.Name = "MyStack";
                       configure.ExchangeOptions.ExchangeType = "topic";
                   },
                   Assembly.GetExecutingAssembly());
                   services.Replace(new ServiceDescriptor(typeof(IMessageSerializer), typeof(CustomJsonMessageSerializer), ServiceLifetime.Transient));
               });

            var app = builder.Build();
            app.Run();
        }
    }
}