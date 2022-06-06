﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;

public partial class FileMetadataParser: IFileMetadataParser
{
    [LoggerMessage(
           EventId = 0,
           Level = LogLevel.Information,
           Message = "Overwriting '{Key}' with '{NewValue}' instead of {OldValue} because '{Reason}'")]
    private partial void LogDataOverwriting(string key, string newValue, string oldValue, string reason);
    private readonly ILogger _logger;
    private readonly IMetadataProvider _metadataProvider;
    private readonly MetadataParserOptions _options;
    public FileMetadataParser(ILogger<FileMetadataParser> logger, IMetadataProvider metadataProvider, MetadataParserOptions options)
    {
        _logger = logger;
        _metadataProvider = metadataProvider;
        _options = options;
    }

    public Metadata<FileMetaData> Parse(MetadataCriteria criteria)
    {
        var result = _metadataProvider.Retrieve<FileMetaData>(criteria.Content);
        var outputLocation = DetermineOutputLocation(criteria.FileName, criteria.Permalink, result.Data);
        var paths = DetermineFilters(outputLocation);

        var fileMetaData = ApplyDefaults(paths, criteria.Scope);
        OverwriteMetaData(fileMetaData, result.Data, "file");
        ApplyDates(fileMetaData);

        // we now have applied all the defaults that match this document and combined it with the retrieved data, store it.
        result.Data = fileMetaData;

        result.Data.Uri = outputLocation;

        return result;
    }

    private string RetrieveExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName);
        if (_options.ExtensionMapping.ContainsKey(ext))
        {
            return _options.ExtensionMapping[ext];
        }
        return ext;
    }

    private FileMetaData ApplyDefaults(List<string> filters, string scope)
    {
        var fileMetaData = new FileMetaData();
        foreach (var filter in filters)
        {
            var defaultMeta = _options.Defaults.DefaultFilter(filter);
            if (defaultMeta != null)
            {
                OverwriteMetaData(fileMetaData, defaultMeta.Values, $"default:{filter}");
            }

            if (!string.IsNullOrEmpty(scope))
            {
                var scopedMeta = _options.Defaults.ScopeFilter(filter, scope);
                if (scopedMeta != null)
                {
                    OverwriteMetaData(fileMetaData, scopedMeta.Values, $"{scope}:{filter}");
                }
            }
        }
        return fileMetaData;
    }

    private static List<string> DetermineFilters(string outputLocation)
    {
        var paths = new List<string>() { string.Empty };
        //var index = outputLocation.LastIndexOf(Path.DirectorySeparatorChar);
        var urlSeperator = "/";
        var index = outputLocation.LastIndexOf(urlSeperator, StringComparison.Ordinal);
        if (index >= 0)
        {
            var input = outputLocation[..index];
            paths.AddRange(DetermineFilterDirectories(input, urlSeperator));
            paths = paths.OrderBy(x => x.Length).ToList();
        }
        return paths;
    }

    private static List<string> DetermineFilterDirectories(string input, string urlSeperator)
    {
        var result = new List<string>();
        int index;
        while ((index = input.LastIndexOf(urlSeperator, StringComparison.Ordinal)) >= 0)
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

    private string DetermineOutputLocation(string fileName, string permalink, FileMetaData metaData)
    {
        var pattern = @"((?<year>\d{4})\-(?<month>\d{2})\-(?<day>\d{2})\-)?(?<filename>[\s\S]*?)\.(?<ext>.*)";
        var match = Regex.Match(fileName, pattern);

        var outputFileName = match.FileNameByPattern();
        var fileDate = match.DateByPattern();
        if (fileDate != null)
        {
            metaData.Date = fileDate;
        }

        var outputExtension = RetrieveExtension(outputFileName);

        var result = permalink
            .Replace("/:year", fileDate == null ? string.Empty : $"/{fileDate?.ToString("yyyy", CultureInfo.InvariantCulture)}")
            .Replace("/:month", fileDate == null ? string.Empty : $"/{fileDate?.ToString("MM", CultureInfo.InvariantCulture)}")
            .Replace("/:day", fileDate == null ? string.Empty : $"/{fileDate?.ToString("dd", CultureInfo.InvariantCulture)}");

        result = result.Replace(":name", Path.GetFileNameWithoutExtension(outputFileName))
            .Replace(":ext", outputExtension);

        if (result.StartsWith("/", StringComparison.Ordinal))
        {
            result = result[1..];
        }
        return result;
        //metaData.Uri = result;
        //metaData.Remove(nameof(metaData.Permalink).ToLower(CultureInfo.InvariantCulture));
    }

    private static void ApplyDates(FileMetaData fileMetaData)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
        ApplyPublishedDates(fileMetaData, tz);
        ApplyModifiedDates(fileMetaData, tz);
        fileMetaData.Remove(nameof(fileMetaData.PublishedDate).ToLower(CultureInfo.InvariantCulture));
        fileMetaData.Remove(nameof(fileMetaData.PublishedTime).ToLower(CultureInfo.InvariantCulture));
        fileMetaData.Remove(nameof(fileMetaData.ModifiedDate).ToLower(CultureInfo.InvariantCulture));
        fileMetaData.Remove(nameof(fileMetaData.ModifiedTime).ToLower(CultureInfo.InvariantCulture));
        fileMetaData.Remove(nameof(fileMetaData.Date).ToLower(CultureInfo.InvariantCulture));
    }

    private static void ApplyPublishedDates(FileMetaData fileMetaData, TimeZoneInfo timeZone)
    {
        if (fileMetaData.Date != null && string.IsNullOrEmpty(fileMetaData.PublishedDate))
        {
            fileMetaData.PublishedDate = fileMetaData.Date.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        if (!string.IsNullOrEmpty(fileMetaData.PublishedDate))
        {
            var dateTimeString = !string.IsNullOrEmpty(fileMetaData.PublishedTime) ? $"{fileMetaData.PublishedDate} {fileMetaData.PublishedTime}" : fileMetaData.PublishedDate;
            var dateTimePattern = !string.IsNullOrEmpty(fileMetaData.PublishedTime) ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd";
            var publishedDateTime = DateTimeOffset.ParseExact(dateTimeString, dateTimePattern, CultureInfo.InvariantCulture).DateTime;
            fileMetaData.Date = null;
            fileMetaData.Published = TimeZoneInfo.ConvertTimeToUtc(publishedDateTime, timeZone);
        }
    }

    private static void ApplyModifiedDates(FileMetaData fileMetaData, TimeZoneInfo timeZone)
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
            var dateTimeString = !string.IsNullOrEmpty(fileMetaData.ModifiedTime) ? $"{fileMetaData.ModifiedDate} {fileMetaData.ModifiedTime}" : fileMetaData.ModifiedDate;
            var dateTimePattern = !string.IsNullOrEmpty(fileMetaData.ModifiedTime) ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd";
            var modifiedDateTime = DateTimeOffset.ParseExact(dateTimeString, dateTimePattern, CultureInfo.InvariantCulture).UtcDateTime;
            fileMetaData.Modified = TimeZoneInfo.ConvertTimeFromUtc(modifiedDateTime, timeZone);;
        }
    }

    private void OverwriteMetaData(FileMetaData target, FileMetaData source, string reason)
    {
        if (source != null)
        {
            foreach (var entry in source)
            {
                if (target.ContainsKey(entry.Key))
                {
                    LogDataOverwriting(entry.Key, (string)entry.Value, (string)target[entry.Key], reason);
                }
                target[entry.Key] = entry.Value;
            }
        }
    }
}
