// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Schema.NET;
using Scriban;
using Scriban.Runtime;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Engine.Transformation.Service;

public class TransformationEngine : ITransformationEngine
{
    private readonly string _layoutDirectory = "_layouts";
    private readonly string _templateDirectory = "_includes";
    private readonly IFileSystem _fileSystem;
    private readonly IMetadataProvider _metadataProvider;
    private readonly ILogger _logger;
    public TransformationEngine(ILogger<TransformationEngine> logger, IFileSystem fileSystem, IMetadataProvider metadataProvider)
    {
        _logger = logger;
        _fileSystem = fileSystem;
        _metadataProvider = metadataProvider;
    }

    private static string ToLdJson(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var settings = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        if (renderData.Page.Type == ContentType.Article)
        {
            var blogPost = new BlogPosting
            {
                // Id = new Uri(GlobalFunctions.AbsoluteUrl(renderData.Page.Url)),
                MainEntityOfPage = new Values<ICreativeWork, Uri>(new Uri(GlobalFunctions.AbsoluteUrl(renderData.Page.Url))),
                Headline = renderData.Page.Title,
                DatePublished = DateTime.Parse((string)renderData.Page["publisheddate"], CultureInfo.InvariantCulture),
                DateModified = DateTime.Parse((string)renderData.Page["modifieddate"], CultureInfo.InvariantCulture),
                Image = new Values<IImageObject, Uri>(new Uri(GlobalFunctions.AbsoluteUrl((string)renderData.Page["image"]))),
                Author = new Values<IOrganization, IPerson>(new Person {
                    Name = new OneOrMany<string>("Max Hamulyák"),
                    Url = new OneOrMany<Uri>(new Uri(GlobalFunctions.Instance.Url))
                }),
                Publisher = new Values<IOrganization, IPerson>(new Organization { })
            };
            return blogPost.ToString(settings);
        }
        return null;
    }

    public async Task<MetadataRenderResult[]> Render(MetadataRenderRequest[] requests)
    {
        var renderedResults = new List<MetadataRenderResult>();
        // TODO apply better solution for access to directories.
        var templates = await new LayoutLoader(_fileSystem, _metadataProvider).Load(Path.Combine("_site", _layoutDirectory)).ConfigureAwait(false);
        var templateLoader = new MyIncludeFromDisk(_fileSystem, Path.Combine("_site", _templateDirectory));

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
                scriptObject.Import("ldjson", () => ToLdJson(request.Metadata));
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
