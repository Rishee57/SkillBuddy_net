namespace SkillBuddy.Worker.Email.Services
{
    public interface ITemplateService
    {
        string LoadTemplate(string templateName);
        string ReplacePlaceholders(string template, Dictionary<string, string> values);
    }
}
