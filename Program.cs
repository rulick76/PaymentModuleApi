using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace PaymentModuleApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Consume();
            CreateHostBuilder(args).Build().Run();
           
        }

        private static void Consume()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("PaymentQueue",true,false,false);
            channel.QueueBind("PaymentQueue","ShoppingCartExchange","");

            var consumer=new EventingBasicConsumer(channel);

            consumer.Received += (sender,eventArgs) => {

                var msg = System.Text.Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                
            };

            channel.BasicConsume("PaymentQueue",true,consumer);

            Console.ReadLine();
            
            channel.Close();
            connection.Close();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
