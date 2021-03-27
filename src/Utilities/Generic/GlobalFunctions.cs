// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Text.Json;
using System.Web;

namespace Kaylumah.Ssg.Utilities
{
    public class GlobalFunctions
    {
        public static readonly GlobalFunctions Instance = new GlobalFunctions();
        public string Url { get; set; }
        public string BaseUrl { get; set; }

        public static string DateToPattern(DateTimeOffset date, string pattern)
        {
            return date.ToUniversalTime().ToString(pattern);
        }

        public static string DateToXmlschema(DateTimeOffset date)
        {
            return date.ToUniversalTime().ToString("o");
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
            var relativeSource = RelativeUrl(source);
            if (relativeSource.StartsWith(Path.DirectorySeparatorChar))
            {
                relativeSource = relativeSource[1..];
            }
            if (!string.IsNullOrWhiteSpace(Instance.Url))
            {
                return Path.Combine(Instance.Url, relativeSource);
            }
            return relativeSource;
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
}