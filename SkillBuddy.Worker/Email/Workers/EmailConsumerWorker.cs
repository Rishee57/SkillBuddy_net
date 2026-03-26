using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SkillBuddy.Worker.Configurations;
using SkillBuddy.Worker.Email.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SkillBuddy.Worker.Email.Workers
{
    public class EmailConsumerWorker : BackgroundService
    {
        private readonly ILogger<EmailConsumerWorker> _logger;
        private readonly RabbitMqOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public EmailConsumerWorker(
            ILogger<EmailConsumerWorker> logger, 
            IOptions<RabbitMqOptions> options,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _options = options.Value;
            _scopeFactory = scopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _options.HostName,
                    UserName = _options.UserName,
                    Password = _options.Password,
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(new CreateChannelOptions(false, false), cancellationToken);

                await _channel.QueueDeclareAsync(
                    queue: _options.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);
                    
                _logger.LogInformation("RabbitMQ Connection established and queue declared.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not initialize RabbitMQ connection.");
            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null) return;
            
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    
                    var emailRequest = JsonSerializer.Deserialize<EmailRequestDto>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (emailRequest != null && !string.IsNullOrEmpty(emailRequest.To))
                    {
                        await emailService.SendEmailAsync(
                            emailRequest.To, 
                            emailRequest.TemplateName ?? "Default", 
                            emailRequest.Placeholders ?? new Dictionary<string, string>(), 
                            stoppingToken);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false, cancellationToken: stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(queue: _options.QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync(cancellationToken);
            if (_connection != null) await _connection.CloseAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }

    public class EmailRequestDto
    {
        public string? To { get; set; }
        public string? TemplateName { get; set; }
        public Dictionary<string, string>? Placeholders { get; set; }
    }
}
