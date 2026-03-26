namespace SkillBuddy.Worker.Configurations
{
    public class RabbitMqOptions
    {
        public const string SectionName = "RabbitMQ";

        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}
