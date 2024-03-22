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
        readonly IFrontMatterMetadataProvider _MetadataProvider;
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
            IFrontMatterMetadataProvider metadataProvider,
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
            criteria.RootDirectory = Constants.Directories.SourceDirectory;
            criteria.DirectoriesToSkip = new[] {
                    Constants.Directories.LayoutDirectory,
                    Constants.Directories.PartialsDirectory,
                    Constants.Directories.DataDirectory,
                    Constants.Directories.AssetDirectory
            };
            criteria.FileExtensionsToTarget = _SiteInfo.SupportedFileExtensions.ToArray();

            IEnumerable<BinaryFile> processed = await _FileProcessor.Process(criteria).ConfigureAwait(false);
            List<BinaryFile> pageList = processed.ToList();
            SiteMetaData siteMetadata = _SiteMetadataFactory.EnrichSite(siteGuid, pageList);

            Artifact[] renderedArtifacts = await GetRenderedArtifacts(siteMetadata);
            Artifact[] generatedArtifacts = GetGeneratedArtifacts(siteMetadata);
            Artifact[] assetArtifacts = GetAssetFolderArtifacts();

            List<Artifact> artifacts = [
                .. renderedArtifacts,
                .. generatedArtifacts,
                .. assetArtifacts
            ];

            OutputLocation outputLocation = new FileSystemOutputLocation(Constants.Directories.DestinationDirectory, false);
            Artifact[] artifactArray = artifacts.ToArray();
            StoreArtifactsRequest storeArtifactsRequest = new StoreArtifactsRequest(outputLocation, artifactArray);
            await _ArtifactAccess.Store(storeArtifactsRequest).ConfigureAwait(false);
        }

        async Task<Artifact[]> GetRenderedArtifacts(SiteMetaData siteMetadata)
        {
            MetadataRenderRequest[] requests = siteMetadata.Items
                            .Select(basePage =>
                            {
                                RenderData metaData = new RenderData(siteMetadata, basePage);
                                string? template = basePage is PageMetaData page ? page.Layout : string.Empty;
                                MetadataRenderRequest result = new MetadataRenderRequest(metaData, template);
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
            directoryConfig.SourceDirectory = Constants.Directories.SourceDirectory;
            directoryConfig.LayoutsDirectory = Constants.Directories.LayoutDirectory;
            directoryConfig.TemplateDirectory = Constants.Directories.PartialsDirectory;
            MetadataRenderResult[] renderResults = await Render(requests).ConfigureAwait(false);

            Artifact[] artifacts = requests.Select((t, i) =>
            {
                MetadataRenderResult renderResult = renderResults[i];
                string artifactPath = $"{t.Metadata.Page.Uri}";
                byte[] bytes = Encoding.UTF8.GetBytes(renderResult.Content);
                Artifact artifact = new Artifact(artifactPath, bytes);
                return artifact;
            }).ToArray();
            return artifacts;
        }

        Artifact[] GetGeneratedArtifacts(SiteMetaData siteMetadata)
        {
            List<Artifact> artifacts = new List<Artifact>();

            foreach (ISiteArtifactPlugin siteArtifactPlugin in _SiteArtifactPlugins)
            {
                Artifact[] pluginArtifacts = siteArtifactPlugin.Generate(siteMetadata);
                artifacts.AddRange(pluginArtifacts);
            }

            Artifact[] result = artifacts.ToArray();
            return result;
        }

        Artifact[] GetAssetFolderArtifacts()
        {
            string assetDirectory = Constants.Directories.SourceAssetsDirectory;
            IEnumerable<IFileSystemInfo> assets = _FileSystem
                .GetFiles(assetDirectory, true)
                .Where(x => !x.IsDirectory());

            string env = Path.Combine(Environment.CurrentDirectory, Constants.Directories.SourceDirectory) + Path.DirectorySeparatorChar;

            IEnumerable<Artifact> assetArtifacts = assets.Select(asset =>
            {
                string assetPath = asset.FullName.Replace(env, "");
                byte[] assetBytes = _FileSystem.GetFileBytes(asset.FullName);
                Artifact artifact = new Artifact(assetPath, assetBytes);
                return artifact;
            });
            Artifact[] artifacts = assetArtifacts.ToArray();
            return artifacts;
        }

        async Task<MetadataRenderResult[]> Render(MetadataRenderRequest[] requests)
        {
            List<MetadataRenderResult> renderedResults = new List<MetadataRenderResult>();
            // TODO apply better solution for access to directories.
            string layoutDirectory = Constants.Directories.SourceLayoutsDirectory;
            List<File<LayoutMetadata>> templates = await new LayoutLoader(_FileSystem, _MetadataProvider).Load(layoutDirectory).ConfigureAwait(false);
            string templateDirectory = Constants.Directories.SourcePartialsDirectory;
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

                    context.MemberRenamer = member =>
                    {
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
}
