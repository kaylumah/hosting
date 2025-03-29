// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public static class FileExtensions
    {
        internal static Dictionary<string, object?> ToDictionary(this TextFile file)
        {
            Dictionary<string, object?> result = new Dictionary<string, object?>(file.MetaData);
            // result.SetValue(nameof(file.LastModified), file.LastModified);
            result.SetValue(nameof(file.Content), file.Content);
            result.SetValue(nameof(file.Name), file.Name);
            return result;
        }

        internal static StaticContent ToStatic(this TextFile file)
        {
            Dictionary<string, object?> data = file.ToDictionary();
            StaticContent result = new StaticContent(data);
            return result;
        }

        internal static PageMetaData ToPage(this TextFile file)
        {
            Dictionary<string, object?> data = file.ToDictionary();
            PageMetaData result = new PageMetaData(data);
            return result;
        }

        internal static ArticleMetaData ToArticle(this TextFile file)
        {
            Dictionary<string, object?> data = file.ToDictionary();
            ArticleMetaData result = new ArticleMetaData(data);
            string content = result.Content;
            (int numberOfWords, TimeSpan duration) readingData = content.ToReadingData();
            result.NumberOfWords = readingData.numberOfWords;
            result.Duration = readingData.duration;
            return result;
        }

        internal static TalkMetaData ToTalk(this TextFile file)
        {
            Dictionary<string, object?> data = file.ToDictionary();
            TalkMetaData result = new TalkMetaData(data);
            return result;
        }

        internal static string ToPageId(this TextFile file, Guid siteGuid)
        {
            Guid pageGuid = file.ToPageGuid(siteGuid);
            string id = pageGuid.ToString();
            return id;
        }

        internal static Guid ToPageGuid(this TextFile file, Guid siteGuid)
        {
            Guid pageGuid = siteGuid.CreatePageGuid(file.MetaData.Uri);
            return pageGuid;
        }

        public static PageMetaData ToPage(this TextFile file, Guid siteGuid)
        {
            PageMetaData page = file.ToPage();
            page.Id = file.ToPageId(siteGuid);
            return page;
        }

        public static ArticleMetaData ToArticle(this TextFile file, Guid siteGuid)
        {
            ArticleMetaData page = file.ToArticle();
            page.Id = file.ToPageId(siteGuid);
            return page;
        }

        public static TalkMetaData ToTalk(this TextFile file, Guid siteGuid)
        {
            TalkMetaData page = file.ToTalk();
            page.Id = file.ToPageId(siteGuid);
            return page;
        }
    }
}
