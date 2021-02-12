using System;
using System.IO;
using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Kaylumah.Ssg.Utilities
{
    public class LiquidUtil
    {
        public string Transform(string source)
        {
            var template = Template.ParseLiquid(source);
            var loader = new MyIncludeFromDisk();
            var context = new LiquidTemplateContext()
            {
                TemplateLoader = loader
            };
            var result = template.Render(context);
            return result;
        }
    }

    internal class MyIncludeFromDisk : ITemplateLoader
    {
        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(Environment.CurrentDirectory, templateName);
        }

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            return File.ReadAllText(templatePath);
        }

        public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            return await File.ReadAllTextAsync(templatePath);
        }
    }
}