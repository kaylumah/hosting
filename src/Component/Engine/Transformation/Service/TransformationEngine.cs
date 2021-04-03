// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Engine.Transformation.Service
{
    public class TransformationEngine : ITransformationEngine
    {
        private readonly string _layoutDirectory = "_layouts";
        private readonly string _templateDirectory = "_includes";
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<IPlugin> _plugins;
        public TransformationEngine(IFileSystem fileSystem, IEnumerable<IPlugin> plugins)
        {
            _fileSystem = fileSystem;
            _plugins = plugins;
        }

        public async Task<MetadataRenderResult[]> Render(MetadataRenderRequest[] requests)
        {
            var renderedResults = new List<MetadataRenderResult>();
            var templates = await new LayoutLoader(_fileSystem).Load(_layoutDirectory);
            var templateLoader = new MyIncludeFromDisk(_fileSystem, _templateDirectory);

            foreach (var request in requests)
            {
                try
                {
                    var template = templates.FirstOrDefault(t => t.Name.Equals(request.Template));
                    var content = template?.Content ?? "{{ content }}";
                    content = content.Replace("{{ content }}", request.Metadata.Content);
                    var liquidTemplate = Template.ParseLiquid(content);
                    var context = new LiquidTemplateContext()
                    {
                        TemplateLoader = templateLoader
                    };
                    var scriptObject = new ScriptObject();
                    scriptObject.Import(request.Metadata);
                    context.PushGlobal(scriptObject);
                    scriptObject.Import(typeof(GlobalFunctions));

                    foreach (var plugin in _plugins)
                    {
                        scriptObject.Import(plugin.Name, new Func<string>(() => plugin.Render(request.Metadata)));
                        // scriptObject.Import(plugin.Name, new Func<string>(() => 
                        //     return plugin.Render(request.Model)
                        // );
                        // scriptObject.Import(plugin.Name, new Func<TemplateContext, string>(templateContext => {
                        //     return plugin.Render(templateContext.CurrentGlobal);
                        // }));
                    }

                    // scriptObject.Import("seo", new Func<TemplateContext, string>(templateContext => {
                    //     return "<strong>{{ build.git_hash }}</strong>";
                    // }));

                    var renderedContent = await liquidTemplate.RenderAsync(context);
                    renderedResults.Add(new MetadataRenderResult { Content = renderedContent });
                }
                catch (Exception)
                {
                    throw;
                }
            }
            
            return renderedResults.ToArray();
        }
    }
}
