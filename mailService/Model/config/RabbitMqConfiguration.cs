namespace mailService.Model.config
{
    public class RabbitMqConfigurationOption
    {
        public string RabbitMqConfiguration { get; private set; } = "RabbitMqConfiguration";
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
