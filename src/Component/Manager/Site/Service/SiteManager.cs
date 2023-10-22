// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Kaylumah.Ssg.Utilities;
using Kaylumah.Ssg.Utilities.Time;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Runtime;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service;

public class SiteManager : ISiteManager
{
    private readonly IArtifactAccess _artifactAccess;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IFileProcessor _fileProcessor;
    private readonly SiteInfo _siteInfo;
    private readonly SiteMetadataFactory _siteMetadataFactory;
    private readonly ISystemClock _systemClock;
    private readonly IMetadataProvider _metadataProvider;
    private readonly IRenderPlugin[] _renderPlugins;
    private readonly ISiteArtifactPlugin[] _siteArtifactPlugins;

    public SiteManager(
        IFileProcessor fileProcessor,
        IArtifactAccess artifactAccess,
        IFileSystem fileSystem,
        ILogger<SiteManager> logger,
        SiteInfo siteInfo,
        SiteMetadataFactory siteMetadataFactory,
        ISystemClock systemClock,
        IMetadataProvider metadataProvider,
        IEnumerable<IRenderPlugin> renderPlugins,
        IEnumerable<ISiteArtifactPlugin> siteArtifactPlugins
        )
    {
        _renderPlugins = renderPlugins.ToArray();
        _siteArtifactPlugins = siteArtifactPlugins.ToArray();
        _siteMetadataFactory = siteMetadataFactory;
        _fileProcessor = fileProcessor;
        _artifactAccess = artifactAccess;
        _fileSystem = fileSystem;
        _logger = logger;
        _siteInfo = siteInfo;
        _systemClock = systemClock;
        _metadataProvider = metadataProvider;
    }

    public async Task GenerateSite(GenerateSiteRequest request)
    {
        GlobalFunctions.Date.Value = _systemClock.LocalNow;
        GlobalFunctions.Url.Value = _siteInfo.Url;
        GlobalFunctions.BaseUrl.Value = _siteInfo.BaseUrl;
        Guid siteGuid = _siteInfo.Url.CreateSiteGuid();

        IEnumerable<Files.Processor.File> processed = await _fileProcessor.Process(new FileFilterCriteria
        {
            RootDirectory = request.Configuration.Source,
            DirectoriesToSkip = new string[] {
                    request.Configuration.LayoutDirectory,
                    request.Configuration.PartialsDirectory,
                    request.Configuration.DataDirectory,
                    request.Configuration.AssetDirectory
                },
            FileExtensionsToTarget = _siteInfo.SupportedFileExtensions.ToArray()
        }).ConfigureAwait(false);

        PageMetaData[] pageMetadatas = processed
            .ToPages(siteGuid);
        SiteMetaData siteMetadata = _siteMetadataFactory
            .EnrichSite(
                request.Configuration,
                siteGuid,
                pageMetadatas.ToList()
        );

        MetadataRenderRequest[] requests = pageMetadatas
            .Select(pageMetadata => new MetadataRenderRequest
            {
                Metadata = new RenderData
                {
                    Site = siteMetadata,
                    Page = pageMetadata
                },
                Template = pageMetadata.Layout
            })
            .ToArray();

        foreach (MetadataRenderRequest renderRequest in requests)
        {
            IRenderPlugin[] plugins = _renderPlugins.Where(plugin => plugin.ShouldExecute(renderRequest.Metadata)).ToArray();
            foreach (IRenderPlugin plugin in plugins)
            {
                plugin.Apply(renderRequest.Metadata);
            }
        }

        DirectoryConfiguration directoryConfig = new DirectoryConfiguration()
        {
            SourceDirectory = request.Configuration.Source,
            LayoutsDirectory = request.Configuration.LayoutDirectory,
            TemplateDirectory = request.Configuration.PartialsDirectory
        };
        MetadataRenderResult[] renderResults = await Render(directoryConfig, requests).ConfigureAwait(false);

        List<Artifact> artifacts = processed.Select((t, i) =>
        {
            MetadataRenderResult renderResult = renderResults[i];
            return new Artifact
            {
                Path = $"{t.MetaData.Uri}",
                Contents = Encoding.UTF8.GetBytes(renderResult.Content)
            };
        }).ToList();

        foreach (ISiteArtifactPlugin siteArtifactPlugin in _siteArtifactPlugins)
        {
            Artifact[] pluginArtifacts = siteArtifactPlugin.Generate(siteMetadata);
            artifacts.AddRange(pluginArtifacts);
        }

        IEnumerable<IFileSystemInfo> assets = _fileSystem
            .GetFiles(Path.Combine(request.Configuration.Source, request.Configuration.AssetDirectory), true)
            .Where(x => !x.IsDirectory());

        string env = Path.Combine(Environment.CurrentDirectory, request.Configuration.Source) + Path.DirectorySeparatorChar;

        artifacts.AddRange(assets.Select(asset =>
        {
            return new Artifact
            {
                Path = asset.FullName.Replace(env, ""),
                Contents = _fileSystem.GetFileBytes(asset.FullName)
            };
        }));

        await _artifactAccess.Store(new StoreArtifactsRequest
        {
            Artifacts = artifacts.ToArray(),
            OutputLocation = new FileSystemOutputLocation()
            {
                Clean = false,
                Path = request.Configuration.Destination
            }
        }).ConfigureAwait(false);
    }

    private async Task<MetadataRenderResult[]> Render(DirectoryConfiguration directoryConfiguration, MetadataRenderRequest[] requests)
    {
        List<MetadataRenderResult> renderedResults = new List<MetadataRenderResult>();
        // TODO apply better solution for access to directories.
        List<File<LayoutMetadata>> templates = await new LayoutLoader(_fileSystem, _metadataProvider).Load(Path.Combine(directoryConfiguration.SourceDirectory, directoryConfiguration.LayoutsDirectory)).ConfigureAwait(false);
        IncludeFromFileSystemTemplateLoader templateLoader = new IncludeFromFileSystemTemplateLoader(_fileSystem, Path.Combine(directoryConfiguration.SourceDirectory, directoryConfiguration.TemplateDirectory));

        foreach (MetadataRenderRequest request in requests)
        {
            try
            {
                File<LayoutMetadata> template = templates.FirstOrDefault(t => t.Name.Equals(request.Template, StringComparison.Ordinal));
                string content = template?.Content ?? "{{ content }}";
                content = content.Replace("{{ content }}", request.Metadata.Content);
                Template liquidTemplate = Template.ParseLiquid(content);
                LiquidTemplateContext context = new LiquidTemplateContext()
                {
                    TemplateLoader = templateLoader
                };
                ScriptObject scriptObject = new ScriptObject();
                scriptObject.Import(request.Metadata);
                // note: work-around for Build becoming part of Site
                scriptObject.Import("build", () => request.Metadata.Site.Build);
                context.PushGlobal(scriptObject);
                scriptObject.Import(typeof(GlobalFunctions));

                // scriptObject.Import("seo", new Func<TemplateContext, string>(templateContext => {
                //     return "<strong>{{ build.git_hash }}</strong>";
                // }));

                string renderedContent = await liquidTemplate.RenderAsync(context).ConfigureAwait(false);
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
