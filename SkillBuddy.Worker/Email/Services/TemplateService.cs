using System;
using System.Collections.Generic;
using System.IO;

namespace SkillBuddy.Worker.Email.Services
{
    public class TemplateService : ITemplateService
    {
        public string LoadTemplate(string templateName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", templateName);
            if (!File.Exists(path))
            {
                return $"Template {templateName} not found. Ensure it exists in the Templates folder.";
            }
            return File.ReadAllText(path);
        }

        public string ReplacePlaceholders(string template, Dictionary<string, string> values)
        {
            if (values == null || string.IsNullOrWhiteSpace(template)) return template;

            foreach (var item in values)
            {
                template = template.Replace($"{{{{{item.Key}}}}}", item.Value);
            }

            return template;
        }
    }
}
