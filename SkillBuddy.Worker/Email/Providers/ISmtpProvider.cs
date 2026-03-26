namespace SkillBuddy.Worker.Email.Providers
{
    public interface ISmtpProvider
    {
        Task SendEmailAsync(string to, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default);
    }
}
