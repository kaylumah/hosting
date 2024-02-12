// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Kaylumah.Ssg.Manager.Site.Interface;
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

        public SiteMetaData EnrichSite(SiteConfiguration siteConfiguration, Guid siteGuid, List<PageMetaData> pages)
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

            siteInfo.Pages = pages.ToList();
            EnrichSiteWithData(siteInfo, siteConfiguration);
            EnrichSiteWithCollections(siteInfo, pages);
            EnrichSiteWithTags(siteInfo, pages);
            EnrichSiteWithYears(siteInfo, pages);
            EnrichSiteWithTypes(siteInfo, pages);
            EnrichSiteWithSeries(siteInfo, pages);

            return siteInfo;
        }
        BuildData EnrichSiteWithAssemblyData()
        {
            LogEnrichSiteWith("AssemblyData");
            AssemblyInfo assemblyInfo = Assembly.GetExecutingAssembly().RetrieveAssemblyInfo();
            DateTimeOffset localNow = _TimeProvider.GetLocalNow();
            BuildData buildMetadata = new BuildData(assemblyInfo, localNow);
            return buildMetadata;
        }

        List<string> GetTags(List<PageMetaData> pages)
        {
            List<string> result = pages.GetTags();
            return result;
        }

        void EnrichSiteWithData(SiteMetaData site, SiteConfiguration siteConfiguration)
        {
            LogEnrichSiteWith("Data");

            string dataDirectory = Path.Combine(siteConfiguration.Source, siteConfiguration.DataDirectory);
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

        void EnrichSiteWithCollections(SiteMetaData site, List<PageMetaData> files)
        {
            LogEnrichSiteWith("Collections");

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
                site.Collections.Add(collection, collectionPages);
            }
        }

        void EnrichSiteWithTags(SiteMetaData site, List<PageMetaData> pages)
        {
            LogEnrichSiteWith("Tags");

            List<string> tags = GetTags(pages);
            foreach (string tag in tags)
            {
                PageMetaData[] tagFiles = pages
                    .FromTag(tag)
                    .ToArray();
                site.Tags.Add(tag, tagFiles);
            }
        }

        void EnrichSiteWithYears(SiteMetaData site, List<PageMetaData> pages)
        {
            LogEnrichSiteWith("Years");
            IEnumerable<int> years = pages
                .IsArticle()
                .Select(x => x.Published.Year)
                .Distinct();
            foreach (int year in years)
            {
                PageMetaData[] yearFiles = pages.Where(x => x.Published.Year.Equals(year)).ToArray();
                site.Years.Add(year, yearFiles);
            }
        }

        void EnrichSiteWithSeries(SiteMetaData site, List<PageMetaData> pages)
        {
            LogEnrichSiteWith("Series");

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
                site.Series.Add(serie, seriesFiles);
            }
        }

        void EnrichSiteWithTypes(SiteMetaData site, List<PageMetaData> pages)
        {
            LogEnrichSiteWith("Types");

            ContentType[] blockedTypes = new ContentType[] { ContentType.Page };
            IEnumerable<ContentType> types = pages
                .Where(x => !blockedTypes.Contains(x.Type))
                .Select(x => x.Type)
                .Distinct();
            foreach (ContentType type in types)
            {
                PageMetaData[] typeFiles = pages.Where(x => /*x.Type != null && */ x.Type.Equals(type)).ToArray();
                string typeName = type.ToString();
                site.Types.Add(typeName, typeFiles);
            }
        }
    }
}
