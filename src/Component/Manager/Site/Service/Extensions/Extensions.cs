// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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

        public static TimeSpan Duration(this string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            double wordsPerMinute = 265;
            double numberOfWords = document.CountWords();
            ;
            int minutes = (int)Math.Ceiling(numberOfWords / wordsPerMinute);
            TimeSpan result = TimeSpan.FromMinutes(minutes);
            return result;
        }
    }
}