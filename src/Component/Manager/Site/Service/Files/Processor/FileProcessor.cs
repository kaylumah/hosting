// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor;

public partial class FileProcessor : IFileProcessor
{
    [LoggerMessage(
       EventId = 0,
       Level = LogLevel.Warning,
       Message = "No files present")]
    private partial void LogNoFiles();

    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IEnumerable<IContentPreprocessorStrategy> _preprocessorStrategies;
    private readonly IFileMetadataParser _fileMetaDataProcessor;
    private readonly SiteInfo _siteInfo;

    public FileProcessor(
        IFileSystem fileSystem,
        ILogger<FileProcessor> logger,
        IEnumerable<IContentPreprocessorStrategy> preprocessorStrategies,
        SiteInfo options,
        IFileMetadataParser fileMetadataParser)
    {
        _siteInfo = options;
        _preprocessorStrategies = preprocessorStrategies;
        _fileSystem = fileSystem;
        _logger = logger;
        _fileMetaDataProcessor = fileMetadataParser;
    }



    public async Task<IEnumerable<File>> Process(FileFilterCriteria criteria)
    {
        var result = new List<File>();

        var directoryContents = _fileSystem.GetFiles("_site").ToList();

        if (!directoryContents.Any())
        {
            LogNoFiles();
            return result;
        }

        var directoriesToProcessAsCollection = directoryContents
            .Where(info => info.IsDirectory() && !criteria.DirectoriesToSkip.Contains(info.Name))
            .ToList();

        var filesWithoutCollections = directoryContents.Where(info =>
            !info.IsDirectory() && criteria.FileExtensionsToTarget.Contains(Path.GetExtension(info.Name))
        ).ToList();

        var files =
            await ProcessFiles(
                filesWithoutCollections
                .Select(x => x.FullName)
                .ToArray()
            ).ConfigureAwait(false);

        result.AddRange(files);

        var collections = await ProcessDirectories(directoriesToProcessAsCollection.Select(x => x.Name).ToArray()).ConfigureAwait(false);
        foreach (var collection in collections)
        {
            var targetFiles = collection
                .Files
                .Where(file => criteria.FileExtensionsToTarget.Contains(Path.GetExtension(file.Name)))
                .ToList();
            var exists = _siteInfo.Collections.Contains(collection.Name);
            if (!exists)
            {
                result.AddRange(targetFiles);
            }
            else
            {
                if (exists && _siteInfo.Collections[collection.Name].Output)
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

    private async Task<List<FileCollection>> ProcessDirectories(string[] collections)
    {
        var result = new List<FileCollection>();
        foreach (var collection in collections)
        {
            using var logScope = _logger.BeginScope($"[ProcessDirectories '{collection}']");
            var keyName = collection[1..];
            var targetFiles = _fileSystem.GetFiles(Path.Combine("_site", collection)).Where(x => !x.IsDirectory()).ToList();
            var files = await ProcessFiles(targetFiles.ToArray(), keyName).ConfigureAwait(false);

            result.Add(new FileCollection
            {
                Name = keyName,
                Files = files.ToArray()
            });
        }
        return result;
    }

    private async Task<List<File>> ProcessFiles(string[] files)
    {
        var fileInfos = new List<IFileInfo>();
        foreach (var file in files)
        {
            fileInfos.Add(_fileSystem.GetFile(file));
        }
        return await ProcessFiles(fileInfos.ToArray(), scope: null).ConfigureAwait(false);
    }

    private async Task<List<File>> ProcessFiles(IFileSystemInfo[] files, string scope)
    {
        var result = new List<File>();
        foreach (var fileInfo in files)
        {
            using var logScope = _logger.BeginScope($"[ProcessFiles '{fileInfo.Name}']");
            var fileStream = fileInfo.CreateReadStream();
            using var streamReader = new StreamReader(fileStream);

            var rawContent = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            var response = _fileMetaDataProcessor.Parse(new MetadataCriteria
            {
                Content = rawContent,
                Scope = scope,
                FileName = fileInfo.Name
            });

            var fileMeta = response.Data;
            var fileContents = response.Content;

            var preprocessor = _preprocessorStrategies.SingleOrDefault(x => x.ShouldExecute(fileInfo));
            if (preprocessor != null)
            {
                fileContents = preprocessor.Execute(fileContents);
            }

            result.Add(new File
            {
                LastModified = fileMeta.Modified ?? fileMeta.Date ?? fileInfo.LastWriteTimeUtc,
                MetaData = fileMeta,
                Content = fileContents,
                Name = Path.GetFileName(fileMeta.Uri)
            });
        }
        return result;
    }
}
