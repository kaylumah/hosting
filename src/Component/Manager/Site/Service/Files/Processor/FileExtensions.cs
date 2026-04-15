// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public static class FileExtensions
    {
        static Dictionary<string, object?> ToDictionary(this TextFile file)
        {
            Dictionary<string, object?> result = new Dictionary<string, object?>(file.MetaData);
            // result.SetValue(nameof(file.LastModified), file.LastModified);
            result.SetValue(nameof(file.Content), file.Content);
            result.SetValue(nameof(file.Name), file.Name);
            return result;
        }
        
        static void SetPageId(this TextFile file, Guid siteGuid)
        {
            Guid pageGuid = file.CreatePageId(siteGuid);
            file.MetaData["Id"] = pageGuid.ToString();
        }

        static Guid CreatePageId(this TextFile file, Guid siteGuid)
        {
            Guid pageGuid = siteGuid.CreatePageGuid(file.MetaData.Uri);
            return pageGuid;
        }

        public static StaticContent ToStatic(this TextFile file)
        {
            Dictionary<string, object?> data = file.ToDictionary();
            StaticContent result = new StaticContent(data);
            return result;
        }
        
        public static PageMetaData ToPage(this TextFile file, Guid siteGuid)
        {
            file.SetPageId(siteGuid);
            Dictionary<string, object?> data = file.ToDictionary();
            PageMetaData page = new PageMetaData(data);
            return page;
        }

        public static ArticlePublicationPageMetaData ToArticle(this TextFile file, Guid siteGuid)
        {
            file.SetPageId(siteGuid);
            Dictionary<string, object?> data = file.ToDictionary();
            ArticlePublicationPageMetaData result = new ArticlePublicationPageMetaData(data);
            string content = result.Content;
            (int numberOfWords, TimeSpan duration) readingData = content.ToReadingData();
            result.NumberOfWords = readingData.numberOfWords;
            result.Duration = readingData.duration;
            return result;
        }

        public static TalkPublicationPageMetaData ToTalk(this TextFile file, Guid siteGuid)
        {
            file.SetPageId(siteGuid);
            Dictionary<string, object?> data = file.ToDictionary();
            TalkPublicationPageMetaData result = new TalkPublicationPageMetaData(data);
            return result;
        }

        public static SnippetThoughtPageMetaData ToSnippet(this TextFile file, Guid siteGuid)
        {
            file.SetPageId(siteGuid);
            Dictionary<string, object?> data = file.ToDictionary();
            SnippetThoughtPageMetaData result = new SnippetThoughtPageMetaData(data);
            return result;
        }
    }
}
