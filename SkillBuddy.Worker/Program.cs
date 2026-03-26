using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SkillBuddy.Worker;
using SkillBuddy.Worker.Configurations;
using SkillBuddy.Worker.Email.Providers;
using SkillBuddy.Worker.Email.Services;
using SkillBuddy.Worker.Email.Workers;
using System;
using System.Threading.Tasks;

var builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configurations
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

// Services Mapping
builder.Services.AddSingleton<ITemplateService, TemplateService>();
builder.Services.AddSingleton<IRetryService, RetryService>();
builder.Services.AddTransient<ISmtpProvider, SmtpProvider>();
builder.Services.AddTransient<IEmailService, EmailService>();

// Workers
builder.Services.AddHostedService<EmailConsumerWorker>();
//builder.Services.AddHostedService<Worker>(); // optional baseline worker

var host = builder.Build();
await host.RunAsync();
