using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SkillBuddy.Worker.Email.Providers;

namespace SkillBuddy.Worker.Email.Services
{
    public class EmailService : IEmailService
    {
        private readonly ITemplateService _templateService;
        private readonly ISmtpProvider _smtpProvider;
        private readonly IRetryService _retryService;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ITemplateService templateService, ISmtpProvider smtpProvider, IRetryService retryService, ILogger<EmailService> logger)
        {
            _templateService = templateService;
            _smtpProvider = smtpProvider;
            _retryService = retryService;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string templateName, Dictionary<string, string> templateValues, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Preparing to send email to {To} using template {TemplateName}", to, templateName);
            
            var templateContent = _templateService.LoadTemplate(templateName);
            var htmlBody = _templateService.ReplacePlaceholders(templateContent, templateValues);
            
            var subject = templateValues.ContainsKey("Subject") ? templateValues["Subject"] : "Notification from SkillBuddy";

            await _retryService.ExecuteAsync(async () => 
            {
                await _smtpProvider.SendEmailAsync(to, subject, "Please view this email in an HTML-compatible client.", htmlBody, cancellationToken);
            });
            
            _logger.LogInformation("Successfully processed sending request for {To}", to);
        }
    }
}
