// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata
{
    public partial class FileParser : IFileParser
    {
        [LoggerMessage(
               EventId = 0,
               Level = LogLevel.Trace,
               Message = "Overwriting '{Key}' with '{NewValue}' instead of {OldValue} because '{Reason}'")]
        private partial void LogDataOverwriting(string key, string newValue, string oldValue, string reason);
        readonly ILogger _Logger;
        readonly IMetadataProvider _MetadataProvider;
        readonly MetadataParserOptions _Options;
        public FileParser(ILogger<FileParser> logger, IMetadataProvider metadataProvider, MetadataParserOptions options)
        {
            _Logger = logger;
            _MetadataProvider = metadataProvider;
            _Options = options;
        }

        public Metadata<FileMetaData> Parse(MetadataCriteria criteria)
        {
            Metadata<FileMetaData> result = _MetadataProvider.Retrieve<FileMetaData>(criteria.Content);
            if (result.Data == null)
            {
                result.Data = new FileMetaData();
            }

            if (string.IsNullOrEmpty(result.Data.OutputLocation))
            {
                result.Data.OutputLocation = "/:year/:month/:day/:name:ext";
            }

            string outputLocation = DetermineOutputLocation(criteria.FileName, result.Data);
            string outputExtension = RetrieveExtension(outputLocation);
            string[] renderExtensions = [".html"];
            bool isMatch = renderExtensions.Contains(outputExtension);

            if (isMatch)
            {
                List<string> paths = DetermineFilters(outputLocation);
                FileMetaData fileMetaData = ApplyDefaults(paths, criteria.Scope);
                OverwriteMetaData(fileMetaData, result.Data, "file");
                ApplyDates(fileMetaData);

                // we now have applied all the defaults that match this document and combined it with the retrieved data, store it.
                result.Data = fileMetaData;
            }

            string lowerName = nameof(result.Data.OutputLocation).ToLower(CultureInfo.InvariantCulture);
            result.Data.Remove(lowerName);
            result.Data.Uri = outputLocation;

            return result;
        }

        string RetrieveExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            if (_Options.ExtensionMapping.TryGetValue(ext, out string? value))
            {
                return value;
            }

            return ext;
        }

        FileMetaData ApplyDefaults(List<string> filters, string scope)
        {
            FileMetaData fileMetaData = new FileMetaData();
            foreach (string filter in filters)
            {
                DefaultMetadata? defaultMeta = _Options.Defaults.DefaultFilter(filter);
                if (defaultMeta != null)
                {
                    OverwriteMetaData(fileMetaData, defaultMeta.Values, $"default:{filter}");
                }

                if (!string.IsNullOrEmpty(scope))
                {
                    DefaultMetadata? scopedMeta = _Options.Defaults.ScopeFilter(filter, scope);
                    if (scopedMeta != null)
                    {
                        OverwriteMetaData(fileMetaData, scopedMeta.Values, $"{scope}:{filter}");
                    }
                }
            }

            return fileMetaData;
        }

        static List<string> DetermineFilters(string outputLocation)
        {
            List<string> paths = new List<string>() { string.Empty };
            //var index = outputLocation.LastIndexOf(Path.DirectorySeparatorChar);
            string urlSeperator = "/";
            int index = outputLocation.LastIndexOf(urlSeperator, StringComparison.Ordinal);
            if (0 <= index)
            {
                string input = outputLocation[..index];
                List<string> filterDirectories = DetermineFilterDirectories(input, urlSeperator);
                paths.AddRange(filterDirectories);
                paths = paths.OrderBy(x => x.Length).ToList();
            }

            return paths;
        }

        static List<string> DetermineFilterDirectories(string input, string urlSeperator)
        {
            List<string> result = new List<string>();
            int index;
            while (0 <= (index = input.LastIndexOf(urlSeperator, StringComparison.Ordinal)))
            {
                result.Add(input);
                input = input[..index];
            }

            if (!string.IsNullOrEmpty(input))
            {
                result.Add(input);
            }

            return result;
        }

        string DetermineOutputLocation(string fileName, FileMetaData metaData)
        {
            string permalink = metaData.OutputLocation;
            string pattern = @"((?<year>\d{4})\-(?<month>\d{2})\-(?<day>\d{2})\-)?(?<filename>[\s\S]*?)\.(?<ext>.*)";
            Match match = Regex.Match(fileName, pattern);

            string outputFileName = match.FileNameByPattern();
            DateTimeOffset? fileDate = match.DateByPattern();
            if (fileDate != null)
            {
                metaData.Date = fileDate;
            }

            string outputExtension = RetrieveExtension(outputFileName);

            string result = permalink
                .Replace("/:year", fileDate == null ? string.Empty : $"/{fileDate?.ToString("yyyy", CultureInfo.InvariantCulture)}")
                .Replace("/:month", fileDate == null ? string.Empty : $"/{fileDate?.ToString("MM", CultureInfo.InvariantCulture)}")
                .Replace("/:day", fileDate == null ? string.Empty : $"/{fileDate?.ToString("dd", CultureInfo.InvariantCulture)}");

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputFileName);
            result = result.Replace(":name", fileNameWithoutExtension)
                .Replace(":ext", outputExtension);

            if (result.StartsWith('/'))
            {
                result = result[1..];
            }

            return result;
            //metaData.Uri = result;
            //metaData.Remove(nameof(metaData.Permalink).ToLower(CultureInfo.InvariantCulture));
        }

        static void ApplyDates(FileMetaData fileMetaData)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
            ApplyPublishedDates(fileMetaData, tz);
            ApplyModifiedDates(fileMetaData, tz);
#pragma warning disable
            fileMetaData.Remove(nameof(fileMetaData.PublishedDate).ToLower(CultureInfo.InvariantCulture));
            fileMetaData.Remove(nameof(fileMetaData.PublishedTime).ToLower(CultureInfo.InvariantCulture));
            fileMetaData.Remove(nameof(fileMetaData.ModifiedDate).ToLower(CultureInfo.InvariantCulture));
            fileMetaData.Remove(nameof(fileMetaData.ModifiedTime).ToLower(CultureInfo.InvariantCulture));
            fileMetaData.Remove(nameof(fileMetaData.Date).ToLower(CultureInfo.InvariantCulture));
#pragma warning restore
        }

        static void ApplyPublishedDates(FileMetaData fileMetaData, TimeZoneInfo timeZone)
        {
            if (fileMetaData.Date != null && string.IsNullOrEmpty(fileMetaData.PublishedDate))
            {
                fileMetaData.PublishedDate = fileMetaData.Date.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(fileMetaData.PublishedDate))
            {
                string dateTimeString = !string.IsNullOrEmpty(fileMetaData.PublishedTime) ? $"{fileMetaData.PublishedDate} {fileMetaData.PublishedTime}" : fileMetaData.PublishedDate;
                string dateTimePattern = !string.IsNullOrEmpty(fileMetaData.PublishedTime) ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd";
                DateTime zonedDateTime = DateTimeOffset.ParseExact(dateTimeString, dateTimePattern, CultureInfo.InvariantCulture).DateTime;
                DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(zonedDateTime, timeZone);
                fileMetaData.Published = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
            }
        }

        static void ApplyModifiedDates(FileMetaData fileMetaData, TimeZoneInfo timeZone)
        {
            if (!string.IsNullOrEmpty(fileMetaData.PublishedDate) && string.IsNullOrEmpty(fileMetaData.ModifiedDate))
            {
                fileMetaData.ModifiedDate = fileMetaData.PublishedDate;
            }

            if (!string.IsNullOrEmpty(fileMetaData.PublishedTime) && string.IsNullOrEmpty(fileMetaData.ModifiedTime))
            {
                fileMetaData.ModifiedTime = fileMetaData.PublishedTime;
            }

            if (!string.IsNullOrEmpty(fileMetaData.ModifiedDate))
            {
                string dateTimeString = !string.IsNullOrEmpty(fileMetaData.ModifiedTime) ? $"{fileMetaData.ModifiedDate} {fileMetaData.ModifiedTime}" : fileMetaData.ModifiedDate;
                string dateTimePattern = !string.IsNullOrEmpty(fileMetaData.ModifiedTime) ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd";
                DateTime zonedDateTime = DateTimeOffset.ParseExact(dateTimeString, dateTimePattern, CultureInfo.InvariantCulture).DateTime;
                DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(zonedDateTime, timeZone);
                fileMetaData.Modified = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
            }
        }

        void OverwriteMetaData(FileMetaData target, FileMetaData source, string reason)
        {
            if (source != null)
            {
                foreach (KeyValuePair<string, object?> entry in source)
                {
#pragma warning disable CA1854
                    if (target.ContainsKey(entry.Key))
                    {
                        LogDataOverwriting(entry.Key, (string)entry.Value!, (string)target[entry.Key]!, reason);
                    }

                    target[entry.Key] = entry.Value;
#pragma warning restore CA1854

                }
            }
        }
    }
}
