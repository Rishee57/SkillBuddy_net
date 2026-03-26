using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace SkillBuddy.ui.Services
{
    public class EmailPublisher
    {
        private readonly IConfiguration _configuration;

        public EmailPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionPropertiesJson()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };

            var connectionInfo = new
            {
                HostName = factory.HostName,
                UserName = factory.UserName,
                VirtualHost = factory.VirtualHost,
                Port = factory.Port
            };

            return JsonSerializer.Serialize(connectionInfo, new JsonSerializerOptions { WriteIndented = true });
        }


        public async Task PublishEmailAsync(EmailMessage emailMessage)
        {
            var factory = new ConnectionFactory
            {
                HostName =  "localhost",//_configuration["RabbitMQ:HostName"] ??
                UserName = "guest",
                VirtualHost = "/",
                Port = 5672

            };

            // Using modern RabbitMQ v7+ async methods
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "email_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var messageJson = JsonSerializer.Serialize(emailMessage);
            var body = Encoding.UTF8.GetBytes(messageJson);

            // In RabbitMQ.Client v7+, BasicPublishAsync has a simplified signature
            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "email_queue",
                mandatory: false,
                basicProperties: properties,
                body: body);
        }
    }

    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
