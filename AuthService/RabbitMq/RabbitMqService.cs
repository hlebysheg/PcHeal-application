using RabbitMQ.Client;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace AuthService.RabbitMq
{
    public class RabbitMqService : IRabbitMqService
    {
        IConfiguration _conf;
        public RabbitMqService(IConfiguration conf)
        {
            _conf = conf;
        }
        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            string host = _conf.GetValue<string>("rabbit:host") ?? "rabbitmq";

            var factory = new ConnectionFactory() { HostName = host };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "mail",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "mail",
                               routingKey: "mail",
                               basicProperties: null,
                               body: body);
            }
        }
    }
}
