using mailService.Model.config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace mailService.Service.RabbitMq
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IConfiguration Configuration;
        public RabbitMqService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConnection CreateChannel()
        {
            var rabbitConf = new RabbitMqConfigurationOption();
            Configuration.GetSection(rabbitConf.RabbitMqConfiguration).Bind(rabbitConf);
            ConnectionFactory connection = new ConnectionFactory()
            {
                UserName = rabbitConf.Username,
                Password = rabbitConf.Password,
                HostName = rabbitConf.HostName
            };
            connection.DispatchConsumersAsync = true;
			IConnection channel = null;

			try
            {
				channel = connection.CreateConnection();
			}
            catch
            {
                return null;
            }
            
            return channel;
        }
    }
}
