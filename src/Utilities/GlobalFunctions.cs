using System;
using System.IO;
using System.Text.Json;

namespace Kaylumah.Ssg.Utilities
{
    public class GlobalFunctions
    {
        public static readonly GlobalFunctions Instance = new GlobalFunctions();
        public string Url { get; set; }
        public string BaseUrl { get; set; }

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
    }
}