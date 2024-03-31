// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using HtmlAgilityPack;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class ObjectConversions
    {
        public static AuthorId AuthorId(string author)
        {
            return author;
        }

        public static string PublishedTimeAgo(SiteMetaData siteMetaData, PageMetaData pageMetaData)
        {
            Debug.Assert(pageMetaData != null);
            Debug.Assert(siteMetaData != null);
            Debug.Assert(siteMetaData.Build != null);
            BuildData buildData = siteMetaData.Build;
            DateTimeOffset now = buildData.Time;
            string result = string.Empty;
            return result;
        }

        public static IEnumerable<object> TagCloud(SiteMetaData site)
        {
            SortedDictionary<string, PageMetaData[]> tags = site.Tags;
            TagMetaDataCollection tagMetaData = site.TagMetaData;
            List<object> result = new List<object>();
            foreach (KeyValuePair<string, PageMetaData[]> item in tags)
            {
                string tag = item.Key;
                string displayName = item.Key;
                PageMetaData[] items = item.Value;
                bool success = tagMetaData.TryGetValue(tag, out TagMetaData? tagData);
                if (success && tagData != null)
                {
                    displayName = tagData.Name;
                }

                object resultForTag = new
                {
                    Id = tag,
                    DisplayName = displayName,
                    Size = items.Length
                };
                result.Add(resultForTag);
            }

            return result;
        }

        public static IEnumerable<Article> ArticlesForTag(SiteMetaData site, string tag, int? take = null)
        {
            ArgumentNullException.ThrowIfNull(site);
            ArgumentNullException.ThrowIfNull(tag);

            bool tagExists = site.Tags.TryGetValue(tag, out PageMetaData[]? resultForTag);
            IEnumerable<Article> result;
            if (tagExists && resultForTag != null)
            {
                IEnumerable<Article> asArticles = resultForTag.OfType<Article>();
                result = asArticles.ByRecentlyPublished();
                if (take != null)
                {
                    result = result.Take(take.Value);
                }
            }
            else
            {
                result = Enumerable.Empty<Article>();
            }

            return result;
        }

        public static string ReadingTime(string content)
        {
            // http://www.craigabbott.co.uk/blog/how-to-calculate-reading-time-like-medium
            //https://stackoverflow.com/questions/12787449/html-agility-pack-removing-unwanted-tags-without-removing-content
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);
            // https://stackoverflow.com/questions/60929281/number-of-words-by-htmlagilitypack
            char[] delimiter = new char[] { ' ' };
            int kelime = 0;
            HtmlNode documentNode = document.DocumentNode;
            HtmlNodeCollection textNodes = documentNode.SelectNodes("//text()");
            IEnumerable<string> innerTexts = textNodes.Select(node => node.InnerText);
            foreach (string text in innerTexts)
            {
                IEnumerable<string> words = text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => Char.IsLetter(s[0]));
                int wordCount = words.Count();
                if (0 < wordCount)
                {
                    kelime += wordCount;
                }
            }

            double wordsPerMinute = 265;
            double numberOfWords = kelime;
            int minutes = (int)Math.Ceiling(numberOfWords / wordsPerMinute);
            return $"{minutes} minute";
        }

        public static string ToJson(object o)
        {
#pragma warning disable CA1869
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string result = JsonSerializer.Serialize(o, options);
            return result;
        }
    }
}