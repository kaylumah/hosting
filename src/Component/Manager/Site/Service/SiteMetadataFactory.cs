// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{

    public partial class SiteMetadataFactory
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Trace,
            Message = "Enrich Site with `{EnrichmentCategory}`")]
        private partial void LogEnrichSiteWith(string enrichmentCategory);

        readonly SiteInfo _SiteInfo;
        readonly IYamlParser _YamlParser;
        readonly IFileSystem _FileSystem;
        readonly ILogger _Logger;

        readonly TimeProvider _TimeProvider;

        public SiteMetadataFactory(TimeProvider timeProvider, SiteInfo siteInfo, IYamlParser yamlParser, IFileSystem fileSystem, ILogger<SiteMetadataFactory> logger)
        {
            _TimeProvider = timeProvider;
            _SiteInfo = siteInfo;
            _YamlParser = yamlParser;
            _FileSystem = fileSystem;
            _Logger = logger;
        }

        public SiteMetaData EnrichSite(Guid siteGuid, List<BinaryFile> files)
        {
            using IDisposable? logScope = _Logger.BeginScope("[EnrichSite]");
            string siteId = siteGuid.ToString();
            BuildData buildData = EnrichSiteWithAssemblyData();
            SiteMetaData siteInfo = new SiteMetaData(siteId,
                _SiteInfo.Title,
                _SiteInfo.Description,
                _SiteInfo.Lang,
                string.Empty,
                _SiteInfo.Url,
                buildData);

            List<TextFile> textFiles = files.OfType<TextFile>().ToList();
            siteInfo.Items = ToPageMetadata(textFiles, siteGuid);
            EnrichSiteWithData(siteInfo);
            // EnrichSiteWithCollections(siteInfo);
            // EnrichSiteWithYears(siteInfo);
            // EnrichSiteWithSeries(siteInfo);

            return siteInfo;
        }

        List<BasePage> ToPageMetadata(IEnumerable<TextFile> files, Guid siteGuid)
        {
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
                IEnumerable<Article> orderedArticlePages = articlePages.ByRecentlyPublished();
                // Article mostRecent = orderedArticlePages.First();
                // Article oldest = orderedArticlePages.Last();

                foreach (TextFile file in collection)
                {
                    // Some parts are regular page data
                    PageMetaData pageMetaData = file.ToPage(siteGuid);

                    CollectionPage collectionPage = new CollectionPage(pageMetaData);
                    result.Add(collectionPage);
                }
            }

            return result;
        }

        BuildData EnrichSiteWithAssemblyData()
        {
            LogEnrichSiteWith("AssemblyData");
            AssemblyInfo assemblyInfo = Assembly.GetExecutingAssembly().RetrieveAssemblyInfo();
            DateTimeOffset localNow = _TimeProvider.GetLocalNow();
            BuildData buildMetadata = new BuildData(assemblyInfo, localNow);
            return buildMetadata;
        }

        void EnrichSiteWithData(SiteMetaData site)
        {
            LogEnrichSiteWith("Data");

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

            List<IKnownFileProcessor> knownFileProcessors =
            [
                new TagFileProcessor(_Logger, _YamlParser),
                new OrganizationFileProcessor(_YamlParser),
                new AuthorFileProcessor(_YamlParser)
            ];

            List<string> knownFileNames = knownFileProcessors.Select(x => x.KnownFileName).ToList();
            List<IFileSystemInfo> knownFiles = dataFiles.Where(file => knownFileNames.Contains(file.Name)).ToList();
            dataFiles = dataFiles.Except(knownFiles).ToList();

            foreach (IFileSystemInfo fileSystemInfo in knownFiles)
            {
                IKnownFileProcessor? strategy = knownFileProcessors.SingleOrDefault(processor => processor.IsApplicable(fileSystemInfo));
                strategy?.Execute(site, fileSystemInfo);
            }

            List<IKnownExtensionProcessor> knownExtensionProcessors =
            [
                new YamlFileProcessor(_YamlParser)
            ];

            foreach (IFileSystemInfo fileSystemInfo in dataFiles)
            {
                IKnownExtensionProcessor? strategy = knownExtensionProcessors.SingleOrDefault(processor => processor.IsApplicable(fileSystemInfo));
                strategy?.Execute(site, fileSystemInfo);
            }
        }

        // void EnrichSiteWithCollections(SiteMetaData site)
        // {
        //     LogEnrichSiteWith("Collections");

        //     List<PageMetaData> files = site.GetPages().ToList();

        //     List<string> collections = files
        //         .Where(x => x.Collection != null)
        //         .Select(x => x.Collection)
        //         .Distinct()
        //         .ToList();

        //     for (int i = collections.Count - 1; 0 < i; i--)
        //     {
        //         string collection = collections[i];
        //         if (_SiteInfo.Collections.Contains(collection))
        //         {
        //             Collection collectionSettings = _SiteInfo.Collections[collection];
        //             if (!string.IsNullOrEmpty(collectionSettings.TreatAs))
        //             {
        //                 if (_SiteInfo.Collections.Contains(collectionSettings.TreatAs))
        //                 {
        //                     // todo log
        //                     IEnumerable<PageMetaData> collectionFiles = files
        //                         .Where(x => x.Collection != null && x.Collection.Equals(collection, StringComparison.Ordinal));
        //                     foreach (PageMetaData file in collectionFiles)
        //                     {
        //                         file.Collection = collectionSettings.TreatAs;
        //                     }

        //                     collections.RemoveAt(i);
        //                 }
        //             }
        //         }
        //     }

        //     foreach (string collection in collections)
        //     {
        //         PageMetaData[] collectionPages = files
        //             .Where(x =>
        //             {
        //                 bool notEmpty = x.Collection != null;
        //                 if (notEmpty)
        //                 {
        //                     bool isMatch = x.Collection!.Equals(collection, StringComparison.Ordinal);
        //                     return isMatch;
        //                 }

        //                 return false;
        //             })
        //             .ToArray();
        //         site.Collections.Add(collection, collectionPages);
        //     }
        // }

        // void EnrichSiteWithYears(SiteMetaData site)
        // {
        //     LogEnrichSiteWith("Years");
        //     List<PageMetaData> pages = site.GetPages().ToList();
        //     IEnumerable<int> years = pages
        //         .IsArticle()
        //         .Select(x => x.Published.Year)
        //         .Distinct();
        //     foreach (int year in years)
        //     {
        //         PageMetaData[] yearFiles = pages.Where(x => x.Published.Year.Equals(year)).ToArray();
        //         site.Years.Add(year, yearFiles);
        //     }
        // }

        // void EnrichSiteWithSeries(SiteMetaData site)
        // {
        //     LogEnrichSiteWith("Series");
        //     List<Article> pages = site.GetArticles().ToList();

        //     IEnumerable<string> series = pages
        //         .HasSeries()
        //         .Select(x => x.Series)
        //         .Distinct();

        //     foreach (string serie in series)
        //     {
        //         PageMetaData[] seriesFiles = pages
        //             .FromSeries(serie)
        //             .OrderBy(x => x.Uri)
        //             .ToArray();
        //         site.Series.Add(serie, seriesFiles);
        //     }
        // }
    }
}