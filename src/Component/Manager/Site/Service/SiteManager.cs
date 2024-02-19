// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
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
        readonly TimeProvider _TimeProvider;
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
            TimeProvider timeProvider,
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
            _TimeProvider = timeProvider;
            _MetadataProvider = metadataProvider;
        }

        public async Task GenerateSite(GenerateSiteRequest request)
        {
            GlobalFunctions.Date.Value = _TimeProvider.GetLocalNow();
            GlobalFunctions.Url.Value = _SiteInfo.Url;
            GlobalFunctions.BaseUrl.Value = _SiteInfo.BaseUrl;
            Guid siteGuid = _SiteInfo.Url.CreateSiteGuid();

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = request.Configuration.Source;
            criteria.DirectoriesToSkip = new[] {
                    request.Configuration.LayoutDirectory,
                    request.Configuration.PartialsDirectory,
                    request.Configuration.DataDirectory,
                    request.Configuration.AssetDirectory
            };
            criteria.FileExtensionsToTarget = _SiteInfo.SupportedFileExtensions.ToArray();

            IEnumerable<Files.Processor.File> processed = await _FileProcessor.Process(criteria).ConfigureAwait(false);
            // Change how mapping is done...
            PageMetaData[] pageMetadatas = processed
                .ToPages(siteGuid);
            List<PageMetaData> pageList = pageMetadatas.ToList();
            SiteMetaData siteMetadata = _SiteMetadataFactory
                .EnrichSite(request.Configuration, siteGuid, pageList);

            MetadataRenderRequest[] requests = pageMetadatas
                .Select(pageMetadata =>
                {
                    RenderData metaData = new RenderData(siteMetadata, pageMetadata);
                    MetadataRenderRequest result = new MetadataRenderRequest(metaData, pageMetadata.Layout);
                    return result;
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

            DirectoryConfiguration directoryConfig = new DirectoryConfiguration();
            directoryConfig.SourceDirectory = request.Configuration.Source;
            directoryConfig.LayoutsDirectory = request.Configuration.LayoutDirectory;
            directoryConfig.TemplateDirectory = request.Configuration.PartialsDirectory;
            MetadataRenderResult[] renderResults = await Render(directoryConfig, requests).ConfigureAwait(false);

            List<Artifact> artifacts = processed.Select((t, i) =>
            {
                MetadataRenderResult renderResult = renderResults[i];
                string artifactPath = $"{t.MetaData.Uri}";
                byte[] bytes = Encoding.UTF8.GetBytes(renderResult.Content);
                Artifact artifact = new Artifact(artifactPath, bytes);
                return artifact;
            }).ToList();

            foreach (ISiteArtifactPlugin siteArtifactPlugin in _SiteArtifactPlugins)
            {
                Artifact[] pluginArtifacts = siteArtifactPlugin.Generate(siteMetadata);
                artifacts.AddRange(pluginArtifacts);
            }

            string assetDirectory = Path.Combine(request.Configuration.Source, request.Configuration.AssetDirectory);
            IEnumerable<IFileSystemInfo> assets = _FileSystem
                .GetFiles(assetDirectory, true)
                .Where(x => !x.IsDirectory());

            string env = Path.Combine(Environment.CurrentDirectory, request.Configuration.Source) + Path.DirectorySeparatorChar;

            IEnumerable<Artifact> assetArtifacts = assets.Select(asset =>
            {
                string assetPath = asset.FullName.Replace(env, "");
                byte[] assetBytes = _FileSystem.GetFileBytes(asset.FullName);
                Artifact artifact = new Artifact(assetPath, assetBytes);
                return artifact;
            });
            artifacts.AddRange(assetArtifacts);

            OutputLocation outputLocation = new FileSystemOutputLocation(request.Configuration.Destination, false);
            Artifact[] artifactArray = artifacts.ToArray();
            StoreArtifactsRequest storeArtifactsRequest = new StoreArtifactsRequest(outputLocation, artifactArray);
            await _ArtifactAccess.Store(storeArtifactsRequest).ConfigureAwait(false);
        }

        async Task<MetadataRenderResult[]> Render(DirectoryConfiguration directoryConfiguration, MetadataRenderRequest[] requests)
        {
            List<MetadataRenderResult> renderedResults = new List<MetadataRenderResult>();
            // TODO apply better solution for access to directories.
            string layoutDirectory = Path.Combine(directoryConfiguration.SourceDirectory, directoryConfiguration.LayoutsDirectory);
            List<File<LayoutMetadata>> templates = await new LayoutLoader(_FileSystem, _MetadataProvider).Load(layoutDirectory).ConfigureAwait(false);
            string templateDirectory = Path.Combine(directoryConfiguration.SourceDirectory, directoryConfiguration.TemplateDirectory);
            IncludeFromFileSystemTemplateLoader templateLoader = new IncludeFromFileSystemTemplateLoader(_FileSystem, templateDirectory);

            foreach (MetadataRenderRequest request in requests)
            {
                try
                {
                    File<LayoutMetadata> template = templates.FirstOrDefault(t => t.Name.Equals(request.Template, StringComparison.Ordinal))!;
                    string content = template?.Content ?? "{{ content }}";
                    content = content.Replace("{{ content }}", request.Metadata.Content);
                    Template liquidTemplate = Template.ParseLiquid(content);
                    LiquidTemplateContext context = new LiquidTemplateContext();

                    context.MemberRenamer = member => {
                        // alternative for the lowercase dictionary
                        string result = member.Name.ToLower(CultureInfo.InvariantCulture);
                        return result;
                    };
                    context.TemplateLoader = templateLoader;
                    ScriptObject scriptObject = new ScriptObject();
                    scriptObject.Import(request.Metadata);
                    // note: work-around for Build becoming part of Site
                    scriptObject.Import("build", () => request.Metadata.Site.Build);
                    context.PushGlobal(scriptObject);
                    scriptObject.Import(typeof(GlobalFunctions));
                    scriptObject.Import(typeof(MyObjectConversions));

                    // scriptObject.Import("seo", new Func<TemplateContext, string>(templateContext => {
                    //     return "<strong>{{ build.git_hash }}</strong>";
                    // }));

                    string renderedContent = await liquidTemplate.RenderAsync(context).ConfigureAwait(false);
                    MetadataRenderResult metadataRenderResult = new MetadataRenderResult(renderedContent);
                    renderedResults.Add(metadataRenderResult);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            MetadataRenderResult[] results = renderedResults.ToArray();
            return results;
        }
    }

    public class MyObjectConversions
    {
        public static AuthorId AuthorId(string author)
        {
            return author;
        }
    }
}
