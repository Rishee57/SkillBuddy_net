using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace SkillBuddy.Worker.Email.Services
{
    public class RetryService : IRetryService
    {
        private readonly AsyncRetryPolicy _retryPolicy;

        public RetryService(ILogger<RetryService> logger)
        {
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        logger.LogWarning(exception, "Operation failed on attempt {RetryCount}. Waiting {TimeSpan} before next retry.", retryCount, timeSpan);
                    });
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            await _retryPolicy.ExecuteAsync(action);
        }
    }
}
