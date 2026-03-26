using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SkillBuddy.Worker.Configurations;
using SkillBuddy.Worker.Email.Providers;

namespace SkillBuddy.Worker.Email.Providers
{
    public class SmtpProvider : ISmtpProvider
    {
        private readonly EmailOptions _options;
        private readonly ILogger<SmtpProvider> _logger;

        public SmtpProvider(IOptions<EmailOptions> options, ILogger<SmtpProvider> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = textBody,
                HtmlBody = htmlBody
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_options.SmtpServer, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
        }
    }
}
