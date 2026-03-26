namespace SkillBuddy.Worker.Email.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string templateName, Dictionary<string, string> templateValues, CancellationToken cancellationToken = default);
    }
}
