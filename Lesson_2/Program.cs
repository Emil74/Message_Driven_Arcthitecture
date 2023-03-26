
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Lesson_2
{
 

    class Program
    {
        static void Main(string[] args)
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

                    // Создаем потребителя сообщений
                    var consumer = new EventingBasicConsumer(channel);

                    // Обработчик получения сообщений
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    };

                    // Начинаем получение сообщений
                    channel.BasicConsume(queue: "hello",
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }

}