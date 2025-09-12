using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.subscriber
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://yhggntqu:AWkgEAWvDoIlC_ZlhCJwfKo-q5q-DEcW@chimpanzee.rmq.cloudamqp.com/yhggntqu");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            #region aciklama
            /*
             1-queue name
             2-true: bağlantı koptuğunda queue verinin kaybolmasını engeller
             3-true: Sadece oluşturulan channel üzerinden bağlanabilir
             4-true:Kuyruğa bağlı olan son subscriber bağlantıyı koparır ise kuyruk düşer.
             */
            //channel.QueueDeclare("hello-queue", true, false, false); //Publisherın kurduğunda emin iseniz kuyruk oluşturma koduna subscriber da oluşturulur kod hata vermez.

            /*
             1- 0 ise istenilen bayutta mesaj gönderebilirsin
             2- mesajlar kaç kaç gelsin
             3- true ise 2. parametredeki sayıyı Subscriber instence sayısına böler ve Subscriberlara  sayıda mesajı tek seferde gönderir
                false ise her bir subscriber'ın tek seferde kaç mesaj alacağını söylemiş oluruz
             */
            #endregion
            var randomQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(randomQueueName, "logs-fanout", "", null);//queuedeclare ile devam edersek ilgili subscriber kaybolsa dahi kuyruk durur
             

            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            /*
             1-queue name
             2-true: rabbitmq subscriber'a bir mesaj gönderdiğinde mesajın doğru işlendiğini kontrol etmeden kuyruktan mesajı siler
             3-
             */
            channel.BasicConsume(randomQueueName,false,consumer);
            Console.WriteLine("Loglar Dinleniyor...");

            consumer.Received+=(object sender, BasicDeliverEventArgs e)=>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Gelen Mesaj: "+message);
                /*
                 * 1- Silinecek olan mesaj
                 * 2- Memory'de işlenmiş ama RabbitMQ'ya gitmemiş başja değerler var ise ondan RabbitMQ'yu haberdar eder
                */
                channel.BasicAck(e.DeliveryTag,false);
            };


            Console.ReadLine();
        }
    }
}
