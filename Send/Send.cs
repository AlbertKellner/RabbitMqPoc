using System;
using RabbitMQ.Client;
using System.Text;

class Send
{
    public static void Main()
    {
        for (int i = 0; i < 30; i++)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.QueueDelete("task_queue", false, false);

                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                
                var message = new StringBuilder();
                var seconds = new Random().Next(1, 20);

                if (i == 4) 
                    seconds = 60;

                message = MessageCompose(i, seconds);

                var body = Encoding.UTF8.GetBytes(message.ToString());

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: "task_queue",
                                     basicProperties: properties,
                                     body: body);

                Console.WriteLine($" Sent {i + 1}", message);
            }
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static StringBuilder MessageCompose(int index, int seconds)
    {
        var message = new StringBuilder();

        var identificador = $"{DateTime.Now.Hour}:{ DateTime.Now.Minute}";

        message.Append($"Mensagem {identificador} - {index + 1}");
        for (int j = 0; j < seconds; j++)
            message.Append(".");

        return message;
    }
}