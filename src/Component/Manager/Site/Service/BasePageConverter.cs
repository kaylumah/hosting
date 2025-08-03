// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public static class BasePageConverter
    {
        public static List<BasePage> ToPageMetadata(IEnumerable<TextFile> files, Guid siteGuid, string baseUrl)
        {
            const string fallback = "Unknown";
            const string collectionType = "Collection";

            foreach (TextFile textFile in files)
            {
                textFile.MetaData.SetValue(nameof(PageMetaData.BaseUri), baseUrl);
            }

            IEnumerable<IGrouping<string, TextFile>> filesGroupedByType = files.GroupBy(file =>
            {
                string? type = file.MetaData.GetValue<string?>("type");
                return type ?? fallback;
            });

            Dictionary<string, List<TextFile>> data = filesGroupedByType
                .ToDictionary(group => group.Key, group => group.ToList());

            List<BasePage> result = new List<BasePage>();

            Dictionary<string, Func<TextFile, BasePage>> pageParsers = new Dictionary<string, Func<TextFile, BasePage>>(StringComparer.OrdinalIgnoreCase);
            pageParsers["Static"] = textFile => textFile.ToStatic();
            pageParsers["Page"] = textFile => textFile.ToPage(siteGuid);
            pageParsers["Announcement"] = textFile => textFile.ToPage(siteGuid);
            pageParsers["Article"] = textFile => textFile.ToArticle(siteGuid);
            pageParsers["Talk"] = textFile => textFile.ToTalk(siteGuid);

            HashSet<string> knownTypes = pageParsers.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
            HashSet<string> seenTypes = data.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
            seenTypes.Remove(collectionType);
            seenTypes.Remove(fallback);
            List<string> unknownTypes = seenTypes.Except(knownTypes).ToList();

            if (0 < unknownTypes.Count)
            {
                throw new InvalidOperationException($"Unmapped metadata types found: {string.Join(", ", unknownTypes)}");
            }

            foreach ((string type, Func<TextFile, BasePage> parser) in pageParsers)
            {
                if (data.TryGetValue(type, out List<TextFile>? filesForType))
                {
                    foreach (TextFile file in filesForType)
                    {
                        BasePage page = parser(file);
                        result.Add(page);
                    }
                }
            }

            if (data.TryGetValue(collectionType, out List<TextFile>? collections))
            {
                IEnumerable<PublicationPageMetaData> publicationMetaDataItems = result.OfType<PublicationPageMetaData>();
                List<PublicationPageMetaData> publicationMetaDatas = publicationMetaDataItems.ToList();
                foreach (TextFile file in collections)
                {
                    PageMetaData pageMetaData = file.ToPage(siteGuid);

                    CollectionPageMetaData collectionPageMetaData = new CollectionPageMetaData(pageMetaData, publicationMetaDatas);
                    result.Add(collectionPageMetaData);
                }
            }

            return result;
        }
    }
}