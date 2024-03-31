// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Kaylumah.Ssg.Utilities
{
    public class GlobalFunctions
    {
        public static AsyncLocal<string> Url
        { get; } = new();
        public static AsyncLocal<string> BaseUrl
        { get; } = new();

        public static string DateToPattern(DateTimeOffset date, string pattern)
        {
            string result = date.ToString(pattern, CultureInfo.InvariantCulture);
            return result;
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
