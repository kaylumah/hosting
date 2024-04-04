// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
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

        internal static PageMetaData ToPage(this TextFile file)
        {
            Dictionary<string, object?> data = file.ToDictionary();
            PageMetaData result = new PageMetaData(data);
            return result;
        }

        internal static Article ToArticle(this TextFile file)
        {
            Dictionary<string, object?> data = file.ToDictionary();
            Article result = new Article(data);
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

        public static Article ToArticle(this TextFile file, Guid siteGuid)
        {
            Article page = file.ToArticle();
            page.Id = file.ToPageId(siteGuid);
            return page;
        }
    }

    public static class Extensions
    {
        public static int CountWords(this string source)
        {
            char[] delimiters = new char[] { ' '/*, '\r', '\n'*/ };
            string[] splitSource = source.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            // int result = splitSource.Length;
            IEnumerable<string> onlyText = splitSource.Where(s => Char.IsLetter(s[0]));
            int result = onlyText.Count();
            return result;
        }

        public static int CountWords(this HtmlDocument document)
        {
            // http://www.craigabbott.co.uk/blog/how-to-calculate-reading-time-like-medium
            // https://stackoverflow.com/questions/12787449/html-agility-pack-removing-unwanted-tags-without-removing-content
            // https://stackoverflow.com/questions/60929281/number-of-words-by-htmlagilitypack
            HtmlNode documentNode = document.DocumentNode;
            HtmlNodeCollection textNodes = documentNode.SelectNodes("//text()");
            IEnumerable<string> innerTexts = textNodes.Select(node => node.InnerText);

            int result = 0;
            foreach (string text in innerTexts)
            {
                int wordCount = text.CountWords();
                result += wordCount;
            }

            return result;
        }
    }
}
