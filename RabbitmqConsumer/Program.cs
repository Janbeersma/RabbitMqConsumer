using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitmqConsumer
{
    static class Program
    {
        static void Main(string[] args)
        {
            //Maakt een connectiefabriek
            var factory = new ConnectionFactory
            {
                //De Uri waarop de docker omgeving staat met het poortnummer waarop RabbitMQ luisterd
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };

            //Maakt een connectie naar een van de endpoints default worden geen parameters meegegeven
            //en de connectie wordt terug gegeven door de EndpointResolverFactory
            using var connection = factory.CreateConnection();

            //Maakt en returned een nieuw kannaal, sessie en model
            using var channel = connection.CreateModel();

            //Benaamd een queue en geeft de configuratie aan voor deze queue
            channel.QueueDeclare("modelgen-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            //Maakt een consumer aan en geef deze de iModel die gemaakt wordt door channel
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };

            channel.BasicConsume("modelgen-queue", true, consumer);

            Console.WriteLine("press x to exit");
            Console.ReadLine();
            
        }
    }
}
