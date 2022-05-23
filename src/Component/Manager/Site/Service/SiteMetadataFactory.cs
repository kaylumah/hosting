// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Reflection;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Options;
using Ssg.Extensions.Data.Yaml;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class SiteMetadataFactory
    {
        private readonly SiteInfo _siteInfo;
        private readonly IYamlParser _yamlParser;
        private readonly IFileSystem _fileSystem;

        public SiteMetadataFactory(SiteInfo siteInfo, IYamlParser yamlParser, IFileSystem fileSystem)
        {
            _siteInfo = siteInfo;
            _yamlParser = yamlParser;
            _fileSystem = fileSystem;
        }

        public SiteMetaData EnrichSite(SiteConfiguration siteConfiguration, Guid siteGuid, List<PageMetaData> pages)
        {
            var siteInfo = new SiteMetaData()
            {
                Id = siteGuid.ToString(),
                Title = _siteInfo.Title,
                Description = _siteInfo.Description,
                Language = _siteInfo.Lang,
                Url = _siteInfo.Url,
                Author = null,
                Data = new Dictionary<string, object>(),
                Tags = new SortedDictionary<string, PageMetaData[]>(),
                Collections = new SortedDictionary<string, PageMetaData[]>(),
                Types = new SortedDictionary<string, PageMetaData[]>(),
                Series = new SortedDictionary<string, PageMetaData[]>(),
                Years = new SortedDictionary<int, PageMetaData[]>()
            };

            var assemblyInfo = new AssemblyUtil()
                .RetrieveAssemblyInfo(Assembly.GetExecutingAssembly());
            var buildMetadata = new BuildData(assemblyInfo);
            siteInfo.Build = buildMetadata;
            siteInfo.Pages = pages
            .Where(file => ".html".Equals(Path.GetExtension(file.Name)))
            .Where(file => !"404.html".Equals(file.Name))
            .Select(x => new
            {
                Url = x["url"],
                x.LastModified,
                Sitemap = x["sitemap"]
            });

            EnrichSiteWithData(siteInfo, Path.Combine("_site", siteConfiguration.DataDirectory));
            EnrichSiteWithCollections(siteInfo, siteGuid, pages);
            EnrichSiteWithTags(siteInfo, pages);
            EnrichSiteWithYears(siteInfo, pages);
            EnrichSiteWithTypes(siteInfo, pages);
            EnrichSiteWithSeries(siteInfo, pages);

            return siteInfo;
        }

         private void EnrichSiteWithData(SiteMetaData site, string dataDirectory)
        {
            var extensions = _siteInfo.SupportedDataFileExtensions.ToArray();
            var dataFiles = _fileSystem.GetFiles(dataDirectory)
                .Where(file => !file.IsDirectory())
                .Where(file => extensions.Contains(Path.GetExtension(file.Name)))
                .ToList();
            var data = new Dictionary<string, object>();

            var tagFile = dataFiles.SingleOrDefault(x => x.Name.Equals("tags.yml"));
            if(tagFile != null)
            {
                dataFiles.Remove(tagFile);
                var stream = tagFile.CreateReadStream();
                using var reader = new StreamReader(stream);
                var raw = reader.ReadToEnd();
                var tagData = _yamlParser.Parse<TagMetaDataCollection>(raw);
                data["tags"] = tagData.ToDictionary();
            }

            foreach (var file in dataFiles)
            {
                var stream = file.CreateReadStream();
                using var reader = new StreamReader(stream);
                var raw = reader.ReadToEnd();
                var result = _yamlParser.Parse<object>(raw);
                data[Path.GetFileNameWithoutExtension(file.Name)] = result;
            }
            site.Data = data;
        }

        private void EnrichSiteWithCollections(SiteMetaData site, Guid siteGuid, List<PageMetaData> files)
        {
            var collections = files
                .Where(x => x.Collection != null)
                .Select(x => x.Collection)
                .Distinct()
                .ToList();

            for (var i = collections.Count - 1; i > 0; i--)
            {
                var collection = collections[i];
                if (_siteInfo.Collections.Contains(collection))
                {
                    var collectionSettings = _siteInfo.Collections[collection];
                    if (!string.IsNullOrEmpty(collectionSettings.TreatAs))
                    {
                        if (_siteInfo.Collections.Contains(collectionSettings.TreatAs))
                        {
                            // todo log
                            var collectionFiles = files
                                .Where(x => x.Collection != null && x.Collection.Equals(collection));
                            foreach (var file in collectionFiles)
                            {
                                file.Collection = collectionSettings.TreatAs;
                            }
                            collections.RemoveAt(i);
                        }
                    }
                }
            }

            foreach (var collection in collections)
            {
                site.Collections.Add(collection,
                    files
                    .Where(x => x.Collection != null
                        && x.Collection.Equals(collection))
                    .ToArray()
                );
            }
        }


        private void EnrichSiteWithTags(SiteMetaData site, List<PageMetaData> pages)
        {
            var tags = pages
                .WhereIsTagged()
                .WhereIsArticle()
                .SelectMany(x => x.Tags)
                .Distinct();
            foreach (var tag in tags)
            {
                var tagFiles = pages
                    .WhereIsTaggedWith(tag)
                    .ToArray();
                site.Tags.Add(tag, tagFiles);
            }
        }

        private void EnrichSiteWithYears(SiteMetaData site, List<PageMetaData> pages)
        {
            var years = pages
                .Where(x => x.ContainsKey("date"))
                .WhereIsArticle()
                .Select(x => ((DateTimeOffset)x["date"]).Year)
                .Distinct();
            foreach (var year in years)
            {
                var yearFiles = pages.Where(x => x.ContainsKey("date") && ((DateTimeOffset)x["date"]).Year.Equals(year)).ToArray();
                site.Years.Add(year, yearFiles);
            }
        }

        private void EnrichSiteWithSeries(SiteMetaData site, List<PageMetaData> pages)
        {
            var series = pages
                .WhereIsSeries()
                .Select(x => x.Series)
                .Distinct();

            foreach (var serie in series)
            {
                var seriesFiles = pages
                    .WhereSeriesIs(serie)
                    .OrderBy(x => x.Url)
                    .ToArray();
                site.Series.Add(serie, seriesFiles);
            }
        }

        private void EnrichSiteWithTypes(SiteMetaData site, List<PageMetaData> pages)
        {
            var blockedTypes = new ContentType[] { ContentType.Unknown, ContentType.Page };
            var types = pages
                .Where(x => !blockedTypes.Contains(x.Type))
                .Select(x => x.Type)
                .Distinct();
            foreach (var type in types)
            {
                var typeFiles = pages.Where(x => /*x.Type != null && */ x.Type.Equals(type)).ToArray();
                site.Types.Add(type.ToString(), typeFiles);
            }
        }
    }
}
