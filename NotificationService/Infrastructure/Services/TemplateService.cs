using NotificationService.Application.Common.Interface.Services;
using System.Text.RegularExpressions;

namespace NotificationService.Infrastructure.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ILogger<TemplateService> _logger;
        private readonly string _templatePath;

        public TemplateService(ILogger<TemplateService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _templatePath = Path.Combine(env.ContentRootPath, "Infrastructure", "Templates", "Email");
        }

        public async Task<string> GetTemplateAsync(string templateName)
        {
            var filePath = Path.Combine(_templatePath, $"{templateName}.html");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Template {templateName} not found");
            }

            return await File.ReadAllTextAsync(filePath);
        }

        public string ReplaceTemplateVariables(string template, Dictionary<string, string> variables)
        {
            foreach (var variable in variables)
            {
                var placeholder = $"{{{{{variable.Key}}}}}";
                template = template.Replace(placeholder, variable.Value);
            }
            return template;
        }
    }

   
}