// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using HtmlAgilityPack;

namespace Kaylumah.Ssg.Manager.Site.Service
{
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

        public static TimeSpan Duration(this int numberOfWords, int wordsPerMinute = 256)
        {
            double numberOf = numberOfWords;
            double wordsPer = wordsPerMinute;
            int minutes = (int)Math.Ceiling(numberOf / wordsPer);
            TimeSpan result = TimeSpan.FromMinutes(minutes);
            return result;
        }

        public static HtmlDocument ToHtmlDocument(this string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        public static (int numberOfWords, TimeSpan duration) ToReadingData(this string html)
        {
            (int numberOfWords, TimeSpan duration) result;
            if (string.IsNullOrEmpty(html))
            {
                result = new(0, TimeSpan.Zero);
            }
            else
            {
                HtmlDocument htmlDocument = html.ToHtmlDocument();
                int numberOfWords = htmlDocument.CountWords();
                TimeSpan duration = numberOfWords.Duration();
                result = new(numberOfWords, duration);
            }

            return result;
        }

        public static string ToReadableDuration(this TimeSpan timeSpan)
        {
            int minutes = timeSpan.Minutes;
            string result = $"{minutes} minute";
            return result;
        }

        public static string ToReadableRelativeTime(this DateTimeOffset now, DateTimeOffset date)
        {
            // https://stackoverflow.com/questions/11/calculate-relative-time-in-c-sharp?page=1&tab=votes#tab-top
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            TimeSpan ts = new TimeSpan(now.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * minute)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }

            if (delta < 2 * minute)
            {
                return "a minute ago";
            }

            if (delta < 45 * minute)
            {
                return ts.Minutes + " minutes ago";
            }

            if (delta < 90 * minute)
            {
                return "an hour ago";
            }

            if (delta < 24 * hour)
            {
                return ts.Hours + " hours ago";
            }

            if (delta < 48 * hour)
            {
                return "yesterday";
            }

            if (delta < 30 * day)
            {
                return ts.Days + " days ago";
            }

            if (delta < 12 * month)
            {
                double input = Math.Floor((double)ts.Days / 30);
                int months = Convert.ToInt32(input);
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                double input = Math.Floor((double)ts.Days / 365);
                int years = Convert.ToInt32(input);
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }
}