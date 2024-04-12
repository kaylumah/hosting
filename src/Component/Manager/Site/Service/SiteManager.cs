// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
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
using Ssg.Extensions.Data.Yaml;
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
        readonly TimeProvider _TimeProvider;
        readonly IFrontMatterMetadataProvider _MetadataProvider;
        readonly IRenderPlugin[] _RenderPlugins;
        readonly ISiteArtifactPlugin[] _SiteArtifactPlugins;
        readonly DataProcessor _DataProcessor;

        public SiteManager(
            IFileProcessor fileProcessor,
            IArtifactAccess artifactAccess,
            IFileSystem fileSystem,
            ILogger<SiteManager> logger,
            SiteInfo siteInfo,
            TimeProvider timeProvider,
            IFrontMatterMetadataProvider metadataProvider,
            IEnumerable<IRenderPlugin> renderPlugins,
            IEnumerable<ISiteArtifactPlugin> siteArtifactPlugins,
            DataProcessor dataProcessor
            )
        {
            _RenderPlugins = renderPlugins.ToArray();
            _SiteArtifactPlugins = siteArtifactPlugins.ToArray();
            _FileProcessor = fileProcessor;
            _ArtifactAccess = artifactAccess;
            _FileSystem = fileSystem;
            _Logger = logger;
            _SiteInfo = siteInfo;
            _TimeProvider = timeProvider;
            _MetadataProvider = metadataProvider;
            _DataProcessor = dataProcessor;
        }

        public async Task GenerateSite(GenerateSiteRequest request)
        {
            GlobalFunctions.Url.Value = _SiteInfo.Url;
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

            List<TextFile> textFiles = pageList.OfType<TextFile>().ToList();
            List<BasePage> pages = ToPageMetadata(textFiles, siteGuid, _SiteInfo.Url);
            BuildData buildData = EnrichSiteWithAssemblyData();

            string siteId = siteGuid.ToString();
            SiteMetaData siteMetadata = new SiteMetaData(siteId,
                _SiteInfo.Title,
                _SiteInfo.Description,
                _SiteInfo.Lang,
                string.Empty,
                _SiteInfo.Url,
                buildData);
            siteMetadata.Items = pages;
            _DataProcessor.EnrichSiteWithData(siteMetadata);
            EnrichSite(siteMetadata);

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
                    scriptObject.Import(typeof(ObjectConversions));

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

        List<BasePage> ToPageMetadata(IEnumerable<TextFile> files, Guid siteGuid, string baseUrl)
        {
            foreach (TextFile textFile in files)
            {
                textFile.MetaData.SetValue(nameof(PageMetaData.BaseUri), baseUrl);
            }

            IEnumerable<IGrouping<string, TextFile>> filesGroupedByType = files.GroupBy(file =>
            {
                string? type = file.MetaData.GetValue<string?>("type");
                return type ?? "unknown";
            });
            Dictionary<string, List<TextFile>> data = filesGroupedByType
                .ToDictionary(group => group.Key, group => group.ToList());

            bool hasArticles = data.TryGetValue("Article", out List<TextFile>? articles);
            bool hasPages = data.TryGetValue("Page", out List<TextFile>? pages);
            bool hasStatics = data.TryGetValue("Static", out List<TextFile>? statics);
            bool hasAnnouncements = data.TryGetValue("Announcement", out List<TextFile>? announcements);
            bool hasCollections = data.TryGetValue("Collection", out List<TextFile>? collection);

            List<TextFile> regularFiles = new List<TextFile>();
            List<TextFile> articleFiles = new List<TextFile>();
            List<TextFile> staticFiles = new List<TextFile>();
            if (hasPages && pages != null)
            {
                regularFiles.AddRange(pages);
            }

            if (hasAnnouncements && announcements != null)
            {
                regularFiles.AddRange(announcements);
            }

            if (hasArticles && articles != null)
            {
                articleFiles.AddRange(articles);
            }

            if (hasStatics && statics != null)
            {
                staticFiles.AddRange(statics);
            }

            List<BasePage> result = new List<BasePage>();

            foreach (TextFile file in regularFiles)
            {
                PageMetaData pageMetaData = file.ToPage(siteGuid);
                result.Add(pageMetaData);
            }

            foreach (TextFile file in articleFiles)
            {
                Article pageMetaData = file.ToArticle(siteGuid);
                result.Add(pageMetaData);
            }

            foreach (TextFile file in staticFiles)
            {
                Dictionary<string, object?> fileAsData = file.ToDictionary();
                StaticContent pageMetaData = new StaticContent(fileAsData);
                result.Add(pageMetaData);
            }

            if (hasCollections && collection != null)
            {
                IEnumerable<Article> articlePages = result.OfType<Article>();

                foreach (TextFile file in collection)
                {
                    // Some parts are regular page data
                    PageMetaData pageMetaData = file.ToPage(siteGuid);

                    CollectionPage collectionPage = new CollectionPage(pageMetaData, articlePages);
                    result.Add(collectionPage);
                }
            }

            return result;
        }

        BuildData EnrichSiteWithAssemblyData()
        {
            AssemblyInfo assemblyInfo = Assembly.GetExecutingAssembly().RetrieveAssemblyInfo();
            DateTimeOffset localNow = _TimeProvider.GetLocalNow();
            BuildData buildMetadata = new BuildData(assemblyInfo, localNow);
            return buildMetadata;
        }

        void EnrichSite(SiteMetaData site)
        {
            EnrichSiteWithYears(site);
            EnrichSiteWithSeries(site);
            EnrichSiteWithCollections(site);
        }

        void EnrichSiteWithCollections(SiteMetaData site)
        {

            List<PageMetaData> files = site.GetPages().ToList();

            List<string> collections = files
                .Where(x => x.Collection != null)
                .Select(x => x.Collection)
                .Distinct()
                .ToList();

            for (int i = collections.Count - 1; 0 < i; i--)
            {
                string collection = collections[i];
                if (_SiteInfo.Collections.Contains(collection))
                {
                    Collection collectionSettings = _SiteInfo.Collections[collection];
                    if (!string.IsNullOrEmpty(collectionSettings.TreatAs))
                    {
                        if (_SiteInfo.Collections.Contains(collectionSettings.TreatAs))
                        {
                            // todo log
                            IEnumerable<PageMetaData> collectionFiles = files
                                .Where(x => x.Collection != null && x.Collection.Equals(collection, StringComparison.Ordinal));
                            foreach (PageMetaData file in collectionFiles)
                            {
                                file.Collection = collectionSettings.TreatAs;
                            }

                            collections.RemoveAt(i);
                        }
                    }
                }
            }

            foreach (string collection in collections)
            {
                PageMetaData[] collectionPages = files
                    .Where(x =>
                    {
                        bool notEmpty = x.Collection != null;
                        if (notEmpty)
                        {
                            bool isMatch = x.Collection!.Equals(collection, StringComparison.Ordinal);
                            return isMatch;
                        }

                        return false;
                    })
                    .ToArray();
                // site.Collections.Add(collection, collectionPages);
            }
        }

        void EnrichSiteWithYears(SiteMetaData site)
        {
            List<PageMetaData> pages = site.GetPages().ToList();
            IEnumerable<int> years = pages
                .IsArticle()
                .Select(x => x.Published.Year)
                .Distinct();
            foreach (int year in years)
            {
                PageMetaData[] yearFiles = pages.Where(x => x.Published.Year.Equals(year)).ToArray();
                // site.Years.Add(year, yearFiles);
            }
        }

        void EnrichSiteWithSeries(SiteMetaData site)
        {
            List<Article> pages = site.GetArticles().ToList();

            IEnumerable<string> series = pages
                .HasSeries()
                .Select(x => x.Series)
                .Distinct();

            foreach (string serie in series)
            {
                PageMetaData[] seriesFiles = pages
                    .FromSeries(serie)
                    .OrderBy(x => x.Uri)
                    .ToArray();
                // site.Series.Add(serie, seriesFiles);
            }
        }
    }
}
