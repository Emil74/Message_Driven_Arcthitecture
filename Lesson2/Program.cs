using RabbitMQ.Client;
using System.Text;

namespace Lesson2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Send();
        }
        public static void Send()
        {
            // Создаем фабрику подключений
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://uconfyiz:l9KeXnBJiG9BiAiUl4rrhp8B-C-uQKKh@cougar.rmq.cloudamqp.com/uconfyiz");
            // Создаем подключение
            using (var connection = factory.CreateConnection())
            {
                // Создаем канал
                using (var channel = connection.CreateModel())
                {
                    // Создаем очередь
                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    // Создаем сообщение
                    string message = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    // Отправляем сообщение в очередь
                    channel.BasicPublish(exchange: "",
                                         routingKey: "hello",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
        }
    }
}