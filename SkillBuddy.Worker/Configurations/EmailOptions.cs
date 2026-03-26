namespace SkillBuddy.Worker.Configurations
{
    public class EmailOptions
    {
        public const string SectionName = "Email";

        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}
