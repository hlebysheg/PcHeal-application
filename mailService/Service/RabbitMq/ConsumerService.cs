﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace mailService.Service.RabbitMq
{
    public class ConsumerService : IConsumerService, IDisposable
    {
        private readonly IModel _model;
        private readonly IConnection _connection;
        public ConsumerService(IRabbitMqService rabbitMqService)
        {
            _connection = rabbitMqService.CreateChannel();
            _model = _connection.CreateModel();
            Connection();

		}
        const string _queueName = "your.queue.name";
        public async Task ReadMessgaes()
        {
            var consumer = new AsyncEventingBasicConsumer(_model);
            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var text = System.Text.Encoding.UTF8.GetString(body);
                Console.WriteLine(text);
                await Task.CompletedTask;
                _model.BasicAck(ea.DeliveryTag, false);
            };
            _model.BasicConsume(_queueName, false, consumer);
            await Task.CompletedTask;
        }
		public void Connection()
		{
            try {
				_model.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
				_model.ExchangeDeclare("mail", ExchangeType.Fanout, durable: true, autoDelete: false);
				_model.QueueBind(_queueName, "mail", string.Empty);
			}
            catch(Exception ex)
            {

            }
			
		}

		public void Dispose()
        {
            if (_model.IsOpen)
                _model.Close();
            if (_connection.IsOpen)
                _connection.Close();
        }
    }
}
