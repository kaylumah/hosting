// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    // [Obsolete("do not use SiteMetadataFactory anymore")]
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
            Debug.Assert(files != null);
            using IDisposable? logScope = _Logger.BeginScope("[EnrichSite]");
            string siteId = siteGuid.ToString();
            // BuildData buildData = EnrichSiteWithAssemblyData();
            SiteMetaData siteInfo = new SiteMetaData(siteId,
                _SiteInfo.Title,
                _SiteInfo.Description,
                _SiteInfo.Lang,
                string.Empty,
                _SiteInfo.Url,
                null!);

            // List<TextFile> textFiles = files.OfType<TextFile>().ToList();
            // siteInfo.Items = ToPageMetadata(textFiles, siteGuid);
            // EnrichSiteWithData(siteInfo);
            // EnrichSiteWithCollections(siteInfo);
            // EnrichSiteWithYears(siteInfo);
            // EnrichSiteWithSeries(siteInfo);

            return siteInfo;
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

    }
}