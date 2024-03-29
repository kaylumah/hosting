﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public partial class FileProcessor : IFileProcessor
    {
        [LoggerMessage(
           EventId = 0,
           Level = LogLevel.Warning,
           Message = "No files present")]
        private partial void LogNoFiles();

        [LoggerMessage(
               EventId = 0,
               Level = LogLevel.Trace,
               Message = "Overwriting '{Key}' with '{NewValue}' instead of {OldValue} because '{Reason}'")]
        private partial void LogDataOverwriting(string key, string newValue, string oldValue, string reason);

        readonly IFileSystem _FileSystem;
        readonly ILogger _Logger;
        readonly IEnumerable<IContentPreprocessorStrategy> _PreprocessorStrategies;
        readonly IFrontMatterMetadataProvider _MetadataProvider;
        readonly MetadataParserOptions _Options;
        readonly SiteInfo _SiteInfo;

        public FileProcessor(
            IFileSystem fileSystem,
            ILogger<FileProcessor> logger,
            IEnumerable<IContentPreprocessorStrategy> preprocessorStrategies,
            SiteInfo options,
            IFrontMatterMetadataProvider metadataProvider,
            MetadataParserOptions metadataParserOptions)
        {
            _SiteInfo = options;
            _PreprocessorStrategies = preprocessorStrategies;
            _FileSystem = fileSystem;
            _Logger = logger;
            _MetadataProvider = metadataProvider;
            _Options = metadataParserOptions;
        }

        public async Task<IEnumerable<BinaryFile>> Process(FileFilterCriteria criteria)
        {
            List<BinaryFile> result = new List<BinaryFile>();

            List<IFileSystemInfo> directoryContents = _FileSystem.GetFiles(criteria.RootDirectory).ToList();

            if (directoryContents.Count == 0)
            {
                LogNoFiles();
                return result;
            }

            List<IFileSystemInfo> directoriesToProcessAsCollection = directoryContents
                .Where(info => info.IsDirectory() && !criteria.DirectoriesToSkip.Contains(info.Name))
                .ToList();

            List<IFileSystemInfo> filesWithoutCollections = directoryContents.Where(info =>
            {
                bool notDirectory = !info.IsDirectory();
                string extension = Path.GetExtension(info.Name);
                bool includesExtension = criteria.FileExtensionsToTarget.Contains(extension);
                bool isMatch = notDirectory && includesExtension;
                return isMatch;
            }).ToList();

            string[] fileNames = filesWithoutCollections.Select(x => x.FullName).ToArray();
            List<BinaryFile> files = await ProcessFiles(fileNames).ConfigureAwait(false);
            result.AddRange(files);

            string[] directories = directoriesToProcessAsCollection.Select(x => x.Name).ToArray();
            List<FileCollection> collections = await ProcessDirectories(criteria, directories).ConfigureAwait(false);
            foreach (FileCollection collection in collections)
            {
                List<BinaryFile> targetFiles = collection
                    .Files
                    .Where(file =>
                    {
                        string extension = Path.GetExtension(file.Name);
                        bool result = criteria.FileExtensionsToTarget.Contains(extension);
                        return result;
                    })
                    .ToList();
                bool exists = _SiteInfo.Collections.Contains(collection.Name);
                if (!exists)
                {
                    result.AddRange(targetFiles);
                }
                else
                {
                    if (exists && _SiteInfo.Collections[collection.Name].Output)
                    {
                        targetFiles = targetFiles
                            .Select(x =>
                            {
                                x.MetaData.Collection = collection.Name;
                                return x;
                            })
                            .ToList();
                        result.AddRange(targetFiles);
                    }
                }
            }

            return result;
        }

        async Task<List<BinaryFile>> ProcessFiles(string[] files)
        {
            List<IFileInfo> fileInfos = new List<IFileInfo>();
            foreach (string file in files)
            {
                IFileInfo fileInfo = _FileSystem.GetFile(file);
                fileInfos.Add(fileInfo);
            }

            IFileInfo[] fileInfosArray = fileInfos.ToArray();
            List<BinaryFile> result = await ProcessFilesInScope(fileInfosArray, scope: null).ConfigureAwait(false);
            return result;
        }

        async Task<List<FileCollection>> ProcessDirectories(FileFilterCriteria criteria, string[] directories)
        {
            List<FileCollection> result = new List<FileCollection>();
            foreach (string directory in directories)
            {
                FileCollection fileCollection = await ProcessDirectory(criteria, directory);
                result.Add(fileCollection);
            }

            return result;
        }

        async Task<FileCollection> ProcessDirectory(FileFilterCriteria criteria, string directory)
        {
            using IDisposable? logScope = _Logger.BeginScope($"[Directory: '{directory}']");
            string keyName = directory[1..];
            string collectionDirectory = Path.Combine(criteria.RootDirectory, directory);
            List<IFileSystemInfo> targetFiles = _FileSystem.GetFiles(collectionDirectory).Where(x => !x.IsDirectory()).ToList();
            IFileSystemInfo[] targetFilesArray = targetFiles.ToArray();
            List<BinaryFile> files = await ProcessFilesInScope(targetFilesArray, keyName).ConfigureAwait(false);

            FileCollection fileCollection = new FileCollection();
            fileCollection.Name = keyName;
            fileCollection.Files = files.ToArray();
            return fileCollection;
        }

        async Task<List<BinaryFile>> ProcessFilesInScope(IFileSystemInfo[] files, string? scope)
        {
            List<BinaryFile> result = new List<BinaryFile>();
            foreach (IFileSystemInfo fileInfo in files)
            {
                BinaryFile fileResult = await ProcessFileInScope(fileInfo, scope);
                result.Add(fileResult);
            }

            return result;
        }

        async Task<BinaryFile> ProcessFileInScope(IFileSystemInfo fileInfo, string? scope)
        {
            using IDisposable? logScope = _Logger.BeginScope($"[File: '{fileInfo.Name}']");
            Stream fileStream = fileInfo.CreateReadStream();
            using StreamReader streamReader = new StreamReader(fileStream);
            string rawContent = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            MetadataCriteria criteria = new MetadataCriteria();
            criteria.Content = rawContent;
            if (scope != null)
            {
                criteria.Scope = scope;
            }

            criteria.FileName = fileInfo.Name;
            ParsedFile<FileMetaData> response = Parse(criteria);

            FileMetaData fileMeta = response.FrontMatter;
            string fileContents = response.Content;

            IContentPreprocessorStrategy? preprocessor = _PreprocessorStrategies.SingleOrDefault(x => x.ShouldExecute(fileInfo));
            if (preprocessor != null)
            {
                fileContents = preprocessor.Execute(fileContents);
            }

            Encoding encoding = fileStream.DetermineEncoding();
            byte[] fileBytes = encoding.GetBytes(fileContents);
            BinaryFile fileResult = new TextFile(fileMeta, fileBytes, encoding.WebName);
            return fileResult;
        }

        internal ParsedFile<FileMetaData> Parse(MetadataCriteria criteria)
        {
            ParsedFile<FileMetaData> result = _MetadataProvider.Retrieve<FileMetaData>(criteria.Content);
            if (result.FrontMatter == null)
            {
                result.FrontMatter = new FileMetaData();
            }

            if (string.IsNullOrEmpty(result.FrontMatter.OutputLocation))
            {
                result.FrontMatter.OutputLocation = "/:year/:month/:day/:name:ext";
            }

            string outputLocation = DetermineOutputLocation(criteria.FileName, result.FrontMatter);
            string outputExtension = RetrieveExtension(outputLocation);

            List<string> paths = DetermineFilters(outputLocation);
            FileMetaData fileMetaData = ApplyDefaults(paths, outputExtension, criteria.Scope);
            OverwriteMetaData(fileMetaData, result.FrontMatter, "file");
            ApplyDates(fileMetaData);

            // we now have applied all the defaults that match this document and combined it with the retrieved data, store it.
            result.FrontMatter = fileMetaData;

            string lowerName = nameof(result.FrontMatter.OutputLocation).ToLower(CultureInfo.InvariantCulture);
            result.FrontMatter.Remove(lowerName);
            result.FrontMatter.Uri = outputLocation;

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

        FileMetaData ApplyDefaults(List<string> filters, string extension, string? scope)
        {
            FileMetaData fileMetaData = new FileMetaData();
            foreach (string filter in filters)
            {
                DefaultMetadata? defaultMeta = _Options.Defaults.DefaultFilter(extension, filter);
                if (defaultMeta != null)
                {
                    OverwriteMetaData(fileMetaData, defaultMeta.Values, $"default:{filter}");
                }

                if (!string.IsNullOrEmpty(scope))
                {
                    DefaultMetadata? scopedMeta = _Options.Defaults.ScopeFilter(extension, filter, scope);
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
