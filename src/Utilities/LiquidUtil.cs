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
    public interface IRenderModel
    {
        string Content { get;set; }
    }
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
        private readonly IFileSystem _fileSystem;
        public LiquidUtil(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task<RenderResult[]> Render(RenderRequest[] requests)
        {
            var renderedResults = new List<RenderResult>();
            var templates = await new LayoutLoader(_fileSystem).Load(_layoutDirectory);
            var templateLoader = new MyIncludeFromDisk(_fileSystem, _templateDirectory);
            foreach(var request in requests)
            {
                try
                {
                    var template = templates.FirstOrDefault(t => t.Name.Equals(request.TemplateName));
                    var content = template?.Content ?? "{{ content }}";
                    // content = content.Replace("{{ content }}", request.Model.Content);
                    var liquidTemplate = Template.ParseLiquid(content);
                    var context = new LiquidTemplateContext()
                    {
                        TemplateLoader = templateLoader
                    };
                    var scriptObject = new ScriptObject();
                    scriptObject.Import(request.Model);
                    context.PushGlobal(scriptObject);
                    var renderedContent = await liquidTemplate.RenderAsync(context);
                    renderedResults.Add(new RenderResult { Content = renderedContent });
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
        private readonly IFileSystem _fileSystem;
        private readonly string _templateFolder;
        public MyIncludeFromDisk(IFileSystem fileSystem, string templateFolder)
        {
            _fileSystem = fileSystem;
            _templateFolder = templateFolder;
        }

        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(_fileSystem.GetFile(_templateFolder).Name, templateName);
            // return Path.Combine(Environment.CurrentDirectory, templateName);
        }

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            using var reader = new StreamReader(_fileSystem.GetFile(templatePath).CreateReadStream());
            return reader.ReadToEnd();
            //return File.ReadAllText(templatePath);
        }

        public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            using var reader = new StreamReader(_fileSystem.GetFile(templatePath).CreateReadStream());
            return await reader.ReadToEndAsync();
            // return await File.ReadAllTextAsync(templatePath);
        }
    }
}