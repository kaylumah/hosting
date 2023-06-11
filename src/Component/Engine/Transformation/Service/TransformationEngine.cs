// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Runtime;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Engine.Transformation.Service;

public class TransformationEngine : ITransformationEngine
{
    private readonly IFileSystem _fileSystem;
    private readonly IMetadataProvider _metadataProvider;
    private readonly ILogger _logger;
    public TransformationEngine(ILogger<TransformationEngine> logger, IFileSystem fileSystem, IMetadataProvider metadataProvider)
    {
        _logger = logger;
        _fileSystem = fileSystem;
        _metadataProvider = metadataProvider;
    }

    public async Task<MetadataRenderResult[]> Render(DirectoryConfiguration directoryConfiguration, MetadataRenderRequest[] requests)
    {
        var renderedResults = new List<MetadataRenderResult>();
        // TODO apply better solution for access to directories.
        var templates = await new LayoutLoader(_fileSystem, _metadataProvider).Load(Path.Combine(directoryConfiguration.SourceDirectory, directoryConfiguration.LayoutsDirectory)).ConfigureAwait(false);
        var templateLoader = new MyIncludeFromDisk(_fileSystem, Path.Combine(directoryConfiguration.SourceDirectory, directoryConfiguration.TemplateDirectory));

        foreach (var request in requests)
        {
            try
            {
                var template = templates.FirstOrDefault(t => t.Name.Equals(request.Template, StringComparison.Ordinal));
                var content = template?.Content ?? "{{ content }}";
                content = content.Replace("{{ content }}", request.Metadata.Content);
                var liquidTemplate = Template.ParseLiquid(content);
                var context = new LiquidTemplateContext()
                {
                    TemplateLoader = templateLoader
                };
                var scriptObject = new ScriptObject();
                scriptObject.Import(request.Metadata);
                // note: work-around for Build becoming part of Site
                scriptObject.Import("build", () => request.Metadata.Site.Build);
                context.PushGlobal(scriptObject);
                scriptObject.Import(typeof(GlobalFunctions));

                // scriptObject.Import("seo", new Func<TemplateContext, string>(templateContext => {
                //     return "<strong>{{ build.git_hash }}</strong>";
                // }));

                var renderedContent = await liquidTemplate.RenderAsync(context).ConfigureAwait(false);
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
