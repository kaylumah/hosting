// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        readonly IPostProcessor[] _PostProcessors;
        readonly IEnumerable<IKnownFileProcessor> _KnownFileProcessors;
        readonly IEnumerable<IKnownExtensionProcessor> _KnownExtensionProcessors;

        public SiteManager(
            IFileProcessor fileProcessor,
            IArtifactAccess artifactAccess,
            IFileSystem fileSystem,
            ILogger<SiteManager> logger,
            SiteInfo siteInfo,
            TimeProvider timeProvider,
            IFrontMatterMetadataProvider metadataProvider,
            IEnumerable<IRenderPlugin> renderPlugins,
            IEnumerable<IPostProcessor> postProcessors,
            IEnumerable<IKnownFileProcessor> knownFileProcessors,
            IEnumerable<IKnownExtensionProcessor> knownExtensionProcessors)
        {
            _KnownFileProcessors = knownFileProcessors;
            _KnownExtensionProcessors = knownExtensionProcessors;
            _RenderPlugins = renderPlugins.ToArray();
            _PostProcessors = postProcessors.ToArray();
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
            Dictionary<string, object> siteData = GetSiteData();
            SiteMetaData siteMetadata = new SiteMetaData(siteId,
                _SiteInfo.Title,
                _SiteInfo.Description,
                _SiteInfo.Lang,
                string.Empty,
                _SiteInfo.Url,
                siteData,
                buildData,
                pages);
            EnrichSite(siteMetadata);

            Artifact[] renderedArtifacts = await GetRenderedArtifacts(siteMetadata);
            Artifact[] generatedArtifacts = GetGeneratedArtifacts(siteMetadata);

            Artifact[] postProcessable = [
                .. renderedArtifacts,
                .. generatedArtifacts
            ];

            foreach (Artifact artifact in postProcessable)
            {
                IPostProcessor? postProcessor = _PostProcessors.SingleOrDefault(x => x.IsApplicable(artifact));
                postProcessor?.Apply(artifact);
            }

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

        Dictionary<string, object> GetSiteData()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            string dataDirectory = Constants.Directories.SourceDataDirectory;
            string[] extensions = _SiteInfo.SupportedDataFileExtensions.ToArray();
            List<IFileSystemInfo> dataFiles = _FileSystem.GetFiles(dataDirectory)
                .Where(file => !file.IsDirectory())
                .Where(file =>
                {
                    string extension = Path.GetExtension(file.Name);
                    bool result = extensions.Contains(extension);
                    return result;
                })
                .ToList();

            List<string> knownFileNames = _KnownFileProcessors.Select(x => x.KnownFileName).ToList();
            List<IFileSystemInfo> knownFiles = dataFiles.Where(file => knownFileNames.Contains(file.Name)).ToList();
            dataFiles = dataFiles.Except(knownFiles).ToList();

            foreach (IFileSystemInfo fileSystemInfo in knownFiles)
            {
                IKnownFileProcessor? strategy = _KnownFileProcessors.SingleOrDefault(processor => processor.IsApplicable(fileSystemInfo));
                strategy?.Execute(result, fileSystemInfo);
            }

            foreach (IFileSystemInfo fileSystemInfo in dataFiles)
            {
                IKnownExtensionProcessor? strategy = _KnownExtensionProcessors.SingleOrDefault(processor => processor.IsApplicable(fileSystemInfo));
                strategy?.Execute(result, fileSystemInfo);
            }

            return result;
        }

        async Task<Artifact[]> GetRenderedArtifacts(SiteMetaData siteMetadata)
        {
            IEnumerable<BasePage> pages = siteMetadata.Items;
            MetadataRenderRequest[] requests = pages
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

            // Ensure Feed(s) are generated first
            ISiteArtifactPlugin feedPlugin = new FeedSiteArtifactPlugin();
            ISiteArtifactPlugin sitemapPlugin = new SiteMapSiteArtifactPlugin();

            List<ISiteArtifactPlugin> plugins = new List<ISiteArtifactPlugin>()
            {
                feedPlugin,
                sitemapPlugin
            };

            foreach (ISiteArtifactPlugin siteArtifactPlugin in plugins)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                Artifact[] pluginArtifacts = siteArtifactPlugin.Generate(siteMetadata);
#pragma warning disable
                _Logger.LogInformation($"The '{siteArtifactPlugin.GetType().Name}' took '{stopwatch.ElapsedMilliseconds}ms' to generate '{pluginArtifacts.Length}' artifact(s)");
#pragma warning restore
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

                    CollectionPage collectionPage = new CollectionPage(pageMetaData, (List<BasePage>)articlePages);
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
            Debug.Assert(site != null);
            /*
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
            */
        }

        void EnrichSiteWithYears(SiteMetaData site)
        {
            Debug.Assert(site != null);
            /*
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
            */
        }

        void EnrichSiteWithSeries(SiteMetaData site)
        {
            Debug.Assert(site != null);
            /*
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
            */
        }
    }
}
