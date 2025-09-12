using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://avahqzcc:iV2YwKIaJGihfx5lN_tiwQMYsseRdPYA@woodpecker.rmq.cloudamqp.com/avahqzcc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            /*
             1-queue name
             2-true: bağlantı koptuğunda queue verinin kaybolmasını engeller
             3-true: Sadece oluşturulan channel üzerinden bağlanabilir
             4-true:Kuyruğa bağlı olan son subscriber bağlantıyı koparır ise kuyruk düşer.
             */
            //channel.QueueDeclare("hello-queue", true, false, false); Kuyruğu publisherdan değiş consumerdan oluşturacağız diye kapadık

            /*
             1- exchange name
             2- true: uygulama restart edildiğinde kaybolmasın

             */
            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"log {x}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("logs-fanout", "", null, messageBody);
                Console.WriteLine($"Mesaj gönderilmiştir:{message} ");
            });

           


           
            Console.ReadLine();

        }
    }
}
