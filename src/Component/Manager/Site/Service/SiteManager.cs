// Copyright (c) Kaylumah, 2024. All rights reserved.
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

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class SiteManager : ISiteManager
    {
        readonly IArtifactAccess _ArtifactAccess;
        readonly IFileSystem _FileSystem;
        readonly ILogger _Logger;
        readonly IFileProcessor _FileProcessor;
        readonly SiteInfo _SiteInfo;
        readonly SiteMetadataFactory _SiteMetadataFactory;
        readonly ISystemClock _SystemClock;
        readonly IMetadataProvider _MetadataProvider;
        readonly IRenderPlugin[] _RenderPlugins;
        readonly ISiteArtifactPlugin[] _SiteArtifactPlugins;

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
            _RenderPlugins = renderPlugins.ToArray();
            _SiteArtifactPlugins = siteArtifactPlugins.ToArray();
            _SiteMetadataFactory = siteMetadataFactory;
            _FileProcessor = fileProcessor;
            _ArtifactAccess = artifactAccess;
            _FileSystem = fileSystem;
            _Logger = logger;
            _SiteInfo = siteInfo;
            _SystemClock = systemClock;
            _MetadataProvider = metadataProvider;
        }

        public async Task GenerateSite(GenerateSiteRequest request)
        {
            GlobalFunctions.Date.Value = _SystemClock.LocalNow;
            GlobalFunctions.Url.Value = _SiteInfo.Url;
            GlobalFunctions.BaseUrl.Value = _SiteInfo.BaseUrl;
            Guid siteGuid = _SiteInfo.Url.CreateSiteGuid();

            IEnumerable<Files.Processor.File> processed = await _FileProcessor.Process(new FileFilterCriteria
            {
                RootDirectory = request.Configuration.Source,
                DirectoriesToSkip = new string[] {
                    request.Configuration.LayoutDirectory,
                    request.Configuration.PartialsDirectory,
                    request.Configuration.DataDirectory,
                    request.Configuration.AssetDirectory
                },
                FileExtensionsToTarget = _SiteInfo.SupportedFileExtensions.ToArray()
            }).ConfigureAwait(false);

            PageMetaData[] pageMetadatas = processed
                .ToPages(siteGuid);
            SiteMetaData siteMetadata = _SiteMetadataFactory
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
                IRenderPlugin[] plugins = _RenderPlugins.Where(plugin => plugin.ShouldExecute(renderRequest.Metadata)).ToArray();
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
                string path = $"{t.MetaData.Uri}";
                byte[] bytes = Encoding.UTF8.GetBytes(renderResult.Content);
                return new Artifact(path, bytes);
            }).ToList();

            foreach (ISiteArtifactPlugin siteArtifactPlugin in _SiteArtifactPlugins)
            {
                Artifact[] pluginArtifacts = siteArtifactPlugin.Generate(siteMetadata);
                artifacts.AddRange(pluginArtifacts);
            }

            IEnumerable<IFileSystemInfo> assets = _FileSystem
                .GetFiles(Path.Combine(request.Configuration.Source, request.Configuration.AssetDirectory), true)
                .Where(x => !x.IsDirectory());

            string env = Path.Combine(Environment.CurrentDirectory, request.Configuration.Source) + Path.DirectorySeparatorChar;

            artifacts.AddRange(assets.Select(asset =>
            {
                string assetPath = asset.FullName.Replace(env, "");
                byte[] bytes = _FileSystem.GetFileBytes(asset.FullName);
                return new Artifact(assetPath, bytes);
            }));

            await _ArtifactAccess.Store(new StoreArtifactsRequest(
                new FileSystemOutputLocation(request.Configuration.Destination, false),
                artifacts.ToArray())).ConfigureAwait(false);
        }

        async Task<MetadataRenderResult[]> Render(DirectoryConfiguration directoryConfiguration, MetadataRenderRequest[] requests)
        {
            List<MetadataRenderResult> renderedResults = new List<MetadataRenderResult>();
            // TODO apply better solution for access to directories.
            List<File<LayoutMetadata>> templates = await new LayoutLoader(_FileSystem, _MetadataProvider).Load(Path.Combine(directoryConfiguration.SourceDirectory, directoryConfiguration.LayoutsDirectory)).ConfigureAwait(false);
            IncludeFromFileSystemTemplateLoader templateLoader = new IncludeFromFileSystemTemplateLoader(_FileSystem, Path.Combine(directoryConfiguration.SourceDirectory, directoryConfiguration.TemplateDirectory));

            foreach (MetadataRenderRequest request in requests)
            {
                try
                {
                    File<LayoutMetadata>? template = templates.FirstOrDefault(t => t.Name.Equals(request.Template, StringComparison.Ordinal));
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
}
