// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text.Json;
using System.Xml;
using HtmlAgilityPack;

namespace Kaylumah.Ssg.Utilities;

public class GlobalFunctions
{
    public static readonly GlobalFunctions Instance = new();
    public string Url { get; set; }
    public string BaseUrl { get; set; }

    public static DateTimeOffset ToDate(string input)
    {
        IFormatProvider culture = new CultureInfo("en-US", true);
        return DateTimeOffset.ParseExact(input, "yyyy-MM-dd", culture);
    }

    public static string ReadingTime(string content)
    {
        // http://www.craigabbott.co.uk/blog/how-to-calculate-reading-time-like-medium
        //https://stackoverflow.com/questions/12787449/html-agility-pack-removing-unwanted-tags-without-removing-content
        var document = new HtmlDocument();
        document.LoadHtml(content);

        // document.DocumentNode.Descendants()
        //     .Where(n => n.Name == "pre")
        //     .ToList()
        //     .ForEach(n => n.Remove());

        // var acceptableTags = new string[] {};
        // var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
        // while (nodes.Count > 0)
        // {
        //     var node = nodes.Dequeue();
        //     var parentNode = node.ParentNode;

        //     if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
        //     {
        //         var childNodes = node.SelectNodes("./*|./text()");

        //         if (childNodes != null)
        //         {
        //             foreach (var child in childNodes)
        //             {
        //                 nodes.Enqueue(child);
        //                 parentNode.InsertBefore(child, node);
        //             }
        //         }

        //         parentNode.RemoveChild(node);

        //     }
        // }

        // var text = document.DocumentNode.InnerHtml;

        // https://stackoverflow.com/questions/60929281/number-of-words-by-htmlagilitypack
        char[] delimiter = new char[] { ' ' };
        int kelime = 0;
        foreach (string text in document.DocumentNode
            .SelectNodes("//text()")
            .Select(node => node.InnerText))
        {
            var words = text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => Char.IsLetter(s[0]));
            int wordCount = words.Count();
            if (wordCount > 0)
            {
                // Console.WriteLine(String.Join(" ", words));
                kelime += wordCount;
            }
        }

        double wordsPerMinute = 265;
        double numberOfWords = kelime;//text.Split(' ').Length;
        var minutes = (int)Math.Ceiling(numberOfWords / wordsPerMinute);
        return $"{minutes} minute";
    }

    public static string DateToAgo(DateTimeOffset date)
    {
        // https://stackoverflow.com/questions/11/calculate-relative-time-in-c-sharp?page=1&tab=votes#tab-top
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        var ts = new TimeSpan(DateTime.UtcNow.Ticks - date.Ticks);
        double delta = Math.Abs(ts.TotalSeconds);

        if (delta < 1 * MINUTE)
            return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
        if (delta < 2 * MINUTE)
            return "a minute ago";
        if (delta < 45 * MINUTE)
            return ts.Minutes + " minutes ago";
        if (delta < 90 * MINUTE)
            return "an hour ago";
        if (delta < 24 * HOUR)
            return ts.Hours + " hours ago";
        if (delta < 48 * HOUR)
            return "yesterday";
        if (delta < 30 * DAY)
            return ts.Days + " days ago";
        if (delta < 12 * MONTH)
        {
            int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months <= 1 ? "one month ago" : months + " months ago";
        }
        else
        {
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }

    }

    public static string DateToPattern(DateTimeOffset date, string pattern)
    {
        // date.ToUniversalTime()
        return date.ToString(pattern, CultureInfo.InvariantCulture);
    }

    public static string ToCdata(string source)
    {
        var settings = new XmlWriterSettings()
        {
            ConformanceLevel = ConformanceLevel.Fragment
        };
        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, settings);
        writer.WriteCData(source);
        writer.Close();
        var text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
        return text;
    }

    public static string DateToXmlschema(DateTimeOffset date)
    {
        return DateToPattern(date, "o");
    }

    public static string RelativeUrl(string source)
    {
        if (!string.IsNullOrWhiteSpace(Instance.BaseUrl))
        {
            return Path.Combine($"{Path.DirectorySeparatorChar}", Instance.BaseUrl, source);
        }
        return source;
    }

    public static string AbsoluteUrl(string source)
    {
        var resolvedSource = RelativeUrl(source);
        var webSeperator = '/';
        if (!string.IsNullOrWhiteSpace(resolvedSource))
        {
            if (resolvedSource.StartsWith(Path.DirectorySeparatorChar) || resolvedSource.StartsWith(webSeperator))
            {
                resolvedSource = resolvedSource[1..];
            }
            if (!string.IsNullOrWhiteSpace(Instance.Url))
            {

                resolvedSource = $"{Instance.Url}{webSeperator}{resolvedSource}";
            }
        }
        return resolvedSource
            .Replace(Path.DirectorySeparatorChar, '/');
    }

    public static string ToJson(object o)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(o, options);
    }

    public static string Encode()
    {
        // https://jekyllrb.com/docs/liquid/filters/#options-for-the-slugify-filter
        // https://stackoverflow.com/questions/17352981/webutility-htmldecode-vs-httputilty-htmldecode
        // https://github.com/scriban/scriban/blob/4ee719b54df1c4f58e4bfc7d863197674d693783/src/Scriban/Functions/HtmlFunctions.cs
        return "";
    }
}
