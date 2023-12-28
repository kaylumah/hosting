// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities;
using Kaylumah.Ssg.Utilities.Time;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public partial class SiteMetadataFactory
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Warning,
            Message = "TagFile is missing `{Tags}`")]
        private partial void LogMissingTags(string tags);

        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Trace,
            Message = "Enrich Site with `{EnrichmentCategory}`")]
        private partial void LogEnrichSiteWith(string enrichmentCategory);

        readonly SiteInfo _SiteInfo;
        readonly IYamlParser _YamlParser;
        readonly IFileSystem _FileSystem;
        readonly ILogger _Logger;

        readonly ISystemClock _SystemClock;

        public SiteMetadataFactory(ISystemClock systemClock, SiteInfo siteInfo, IYamlParser yamlParser, IFileSystem fileSystem, ILogger<SiteMetadataFactory> logger)
        {
            _SystemClock = systemClock;
            _SiteInfo = siteInfo;
            _YamlParser = yamlParser;
            _FileSystem = fileSystem;
            _Logger = logger;
        }

        public SiteMetaData EnrichSite(SiteConfiguration siteConfiguration, Guid siteGuid, List<PageMetaData> pages)
        {
            using IDisposable logScope = _Logger.BeginScope("[EnrichSite]");
            SiteMetaData siteInfo = new SiteMetaData()
            {
                Id = siteGuid.ToString(),
                Title = _SiteInfo.Title,
                Description = _SiteInfo.Description,
                Language = _SiteInfo.Lang,
                Url = _SiteInfo.Url,
                Author = null,
                Data = new Dictionary<string, object>(),
                Tags = new SortedDictionary<string, PageMetaData[]>(),
                Collections = new SortedDictionary<string, PageMetaData[]>(),
                Types = new SortedDictionary<string, PageMetaData[]>(),
                Series = new SortedDictionary<string, PageMetaData[]>(),
                Years = new SortedDictionary<int, PageMetaData[]>()
            };
            EnrichSiteWithAssemblyData(siteInfo);
            siteInfo.Pages = pages.ToList();
            EnrichSiteWithData(siteInfo, pages, siteConfiguration);
            EnrichSiteWithCollections(siteInfo, pages);
            EnrichSiteWithTags(siteInfo, pages);
            EnrichSiteWithYears(siteInfo, pages);
            EnrichSiteWithTypes(siteInfo, pages);
            EnrichSiteWithSeries(siteInfo, pages);

            return siteInfo;
        }
        void EnrichSiteWithAssemblyData(SiteMetaData site)
        {
            LogEnrichSiteWith("AssemblyData");
            AssemblyInfo assemblyInfo = Assembly.GetExecutingAssembly().RetrieveAssemblyInfo();
            BuildData buildMetadata = new BuildData(assemblyInfo, _SystemClock.LocalNow);
            site.Build = buildMetadata;
        }

        void EnrichSiteWithData(SiteMetaData site, List<PageMetaData> pages, SiteConfiguration siteConfiguration)
        {
            LogEnrichSiteWith("Data");

            string dataDirectory = Path.Combine(siteConfiguration.Source, siteConfiguration.DataDirectory);
            string[] extensions = _SiteInfo.SupportedDataFileExtensions.ToArray();
            List<IFileSystemInfo> dataFiles = _FileSystem.GetFiles(dataDirectory)
                .Where(file => !file.IsDirectory())
                .Where(file => extensions.Contains(Path.GetExtension(file.Name)))
                .ToList();

            IFileSystemInfo tagFile = dataFiles.SingleOrDefault(x => x.Name.Equals("tags.yml", StringComparison.Ordinal));
            if (tagFile != null)
            {
                dataFiles.Remove(tagFile);
                TagMetaDataCollection tagData = _YamlParser.Parse<TagMetaDataCollection>(tagFile);
                List<string> tags = pages.SelectMany(x => x.Tags).Distinct().ToList();
                IEnumerable<string> unmatchedTags = tags
                    .Except(tagData.Keys)
                    .Concat(tagData.Keys.Except(tags));
                LogMissingTags(string.Join(",", unmatchedTags));
                site.TagMetaData.AddRange(tagData);
                site.Data["tags"] = site.TagMetaData.Dictionary;
            }

            IFileSystemInfo authorFile = dataFiles.SingleOrDefault(x => x.Name.Equals("authors.yml", StringComparison.Ordinal));
            if (authorFile != null)
            {
                dataFiles.Remove(authorFile);
                AuthorMetaDataCollection authorData = _YamlParser.Parse<AuthorMetaDataCollection>(authorFile);
                site.AuthorMetaData.AddRange(authorData);
                site.Data["authors"] = site.AuthorMetaData.Dictionary;
            }

            IFileSystemInfo organizationFile = dataFiles.SingleOrDefault(x => x.Name.Equals("organizations.yml", StringComparison.Ordinal));
            if (organizationFile != null)
            {
                dataFiles.Remove(organizationFile);
                OrganizationMetaDataCollection organizationData = _YamlParser.Parse<OrganizationMetaDataCollection>(organizationFile);
                site.OrganizationMetaData.AddRange(organizationData);
                site.Data["organizations"] = site.OrganizationMetaData.Dictionary;
            }

            EnrichSiteWithData(site, dataFiles);
        }

        void EnrichSiteWithData(SiteMetaData site, List<IFileSystemInfo> dataFiles)
        {
            foreach (IFileSystemInfo file in dataFiles)
            {
                object result = _YamlParser.Parse<object>(file);
                site.Data[Path.GetFileNameWithoutExtension(file.Name)] = result;
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

            for (int i = collections.Count - 1; i > 0; i--)
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
                site.Collections.Add(collection,
                    files
                    .Where(x => x.Collection != null
                        && x.Collection.Equals(collection, StringComparison.Ordinal))
                    .ToArray()
                );
            }
        }

        void EnrichSiteWithTags(SiteMetaData site, List<PageMetaData> pages)
        {
            LogEnrichSiteWith("Tags");

            List<string> tags = pages
                .HasTag()
                .IsArticle()
                .SelectMany(x => x.Tags)
                .Distinct()
                .ToList();
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

            ContentType[] blockedTypes = new ContentType[] { ContentType.Unknown, ContentType.Page };
            IEnumerable<ContentType> types = pages
                .Where(x => !blockedTypes.Contains(x.Type))
                .Select(x => x.Type)
                .Distinct();
            foreach (ContentType type in types)
            {
                PageMetaData[] typeFiles = pages.Where(x => /*x.Type != null && */ x.Type.Equals(type)).ToArray();
                site.Types.Add(type.ToString(), typeFiles);
            }
        }
    }
}
