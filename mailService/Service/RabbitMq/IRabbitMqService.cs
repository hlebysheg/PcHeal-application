using RabbitMQ.Client;

namespace mailService.Service.RabbitMq
{
    public interface IRabbitMqService
    {
        IConnection CreateChannel();
    }
}
