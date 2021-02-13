using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Kaylumah.Ssg.Utilities
{
    public class RenderRequest
    {
        public object Model { get;set; }
        public string TemplateName { get;set; }
    }

    public class RenderResult
    {
        public string Content { get;set; }
    }

    public class LiquidUtil
    {
        private readonly string _layoutDirectory = "_layouts";
        private readonly string _templateDirectory = "_includes";
        private readonly IFileProvider _fileProvider;
        public LiquidUtil(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public async Task<RenderResult[]> Render(RenderRequest[] requests)
        {
            var renderedResults = new List<RenderResult>();
            var templates = await new LayoutLoader(_fileProvider).Load(_layoutDirectory);
            var templateLoader = new MyIncludeFromDisk(_fileProvider, _templateDirectory);
            foreach(var request in requests)
            {
                try
                {
                    var template = templates.Single(t => t.Name.Equals(request.TemplateName));
                    var liquidTemplate = Template.ParseLiquid(template.Content);
                    var context = new LiquidTemplateContext()
                    {
                        TemplateLoader = templateLoader
                    };
                    var renderedContent = await liquidTemplate.RenderAsync(context);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return renderedResults.ToArray();
        }
    }

    internal class MyIncludeFromDisk : ITemplateLoader
    {
        private readonly IFileProvider _fileProvider;
        private readonly string _templateFolder;
        public MyIncludeFromDisk(IFileProvider fileProvider, string templateFolder)
        {
            _fileProvider = fileProvider;
            _templateFolder = templateFolder;
        }

        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(_fileProvider.GetFileInfo(_templateFolder).Name, templateName);
            // return Path.Combine(Environment.CurrentDirectory, templateName);
        }

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            using var reader = new StreamReader(_fileProvider.GetFileInfo(templatePath).CreateReadStream());
            return reader.ReadToEnd();
            //return File.ReadAllText(templatePath);
        }

        public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            using var reader = new StreamReader(_fileProvider.GetFileInfo(templatePath).CreateReadStream());
            return await reader.ReadToEndAsync();
            // return await File.ReadAllTextAsync(templatePath);
        }
    }
}