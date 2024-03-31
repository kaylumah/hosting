// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace Kaylumah.Ssg.Utilities
{
    public class GlobalFunctions
    {
        public static AsyncLocal<DateTimeOffset> Date
        { get; } = new();
        public static AsyncLocal<string> Url
        { get; } = new();
        public static AsyncLocal<string> BaseUrl
        { get; } = new();
        public static DateTimeOffset ToDate(string input)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTimeOffset result = DateTimeOffset.ParseExact(input, "yyyy-MM-dd", culture);
            return result;
        }

        public static string DateToAgo(DateTimeOffset date)
        {
            // https://stackoverflow.com/questions/11/calculate-relative-time-in-c-sharp?page=1&tab=votes#tab-top
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            TimeSpan ts = new TimeSpan(Date.Value.Ticks - date.Ticks);
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

        public static string DateToPattern(DateTimeOffset date, string pattern)
        {
            // date.ToUniversalTime()
            string result = date.ToString(pattern, CultureInfo.InvariantCulture);
            return result;
        }

        public static string ToCdata(string source)
        {
#pragma warning disable IDESIGN103
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                Encoding = new System.Text.UTF8Encoding(false)
            };
#pragma warning restore IDESIGN103
            using MemoryStream stream = new MemoryStream();
            using XmlWriter writer = XmlWriter.Create(stream, settings);
            writer.WriteCData(source);
            writer.Close();
            byte[] arr = stream.ToArray();
            string text = System.Text.Encoding.UTF8.GetString(arr);
            return text;
        }

        public static string DateToXmlschema(DateTimeOffset date)
        {
            string result = DateToPattern(date, "o");
            return result;
        }

        public static string FileNameWithoutExtension(string source)
        {
            string extension = Path.GetExtension(source);
            string filePathWithoutExt = source.Substring(0, source.Length - extension.Length);
            return filePathWithoutExt;
        }

        static string RelativeUrl(string source)
        {
            if (!string.IsNullOrWhiteSpace(BaseUrl.Value))
            {
                string result = Path.Combine($"{Path.DirectorySeparatorChar}", BaseUrl.Value, source);
                return result;
            }

            return source;
        }

        static string AbsoluteUrl(string source)
        {
            string resolvedSource = RelativeUrl(source);
            char webSeperator = '/';
            if (!string.IsNullOrWhiteSpace(resolvedSource))
            {
                if (resolvedSource.StartsWith(Path.DirectorySeparatorChar) || resolvedSource.StartsWith(webSeperator))
                {
                    resolvedSource = resolvedSource[1..];
                }

                if (!string.IsNullOrWhiteSpace(Url.Value))
                {

                    resolvedSource = $"{Url.Value}{webSeperator}{resolvedSource}";
                }
            }

            string result = resolvedSource.Replace(Path.DirectorySeparatorChar, '/');
            return result;
        }

        public static Uri AbsoluteUri(string source)
        {
            string absoluteUrl = AbsoluteUrl(source);
            Uri result = new Uri(absoluteUrl);
            return result;
        }
    }
}
