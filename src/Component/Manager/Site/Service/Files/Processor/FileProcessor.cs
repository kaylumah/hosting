// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IEnumerable<IContentPreprocessorStrategy> _preprocessorStrategies;
        private readonly IFileMetadataParser _fileMetaDataProcessor;
        private readonly SiteInfo _siteInfo;

        public FileProcessor(
            IFileSystem fileSystem,
            ILogger<FileProcessor> logger,
            IEnumerable<IContentPreprocessorStrategy> preprocessorStrategies,
            IOptions<SiteInfo> options,
            IFileMetadataParser fileMetadataParser)
        {
            _siteInfo = options.Value;
            _preprocessorStrategies = preprocessorStrategies;
            _fileSystem = fileSystem;
            _logger = logger;
            _fileMetaDataProcessor = fileMetadataParser;
        }

        public async Task<IEnumerable<File>> Process(FileFilterCriteria criteria)
        {
            var result = new List<File>();

            var directoryContents = _fileSystem.GetDirectoryContents(string.Empty);
            if (directoryContents.Count() > 0)
            {
                var directoriesToProcessAsCollection = directoryContents
                    .Where(info => info.IsDirectory && !criteria.DirectoriesToSkip.Contains(info.Name));
                var filesWithoutCollections = directoryContents.Where(info =>
                    !info.IsDirectory && criteria.FileExtensionsToTarget.Contains(Path.GetExtension(info.Name))
                );

                var files =
                    await ProcessFiles(
                        filesWithoutCollections
                        .Select(x => x.Name)
                        .ToArray()
                    );

                result.AddRange(files);

                var collections = await ProcessDirectories(directoriesToProcessAsCollection.Select(x => x.Name).ToArray());
                foreach (var collection in collections)
                {
                    var targetFiles = collection
                        .Files
                        .Where(file => criteria.FileExtensionsToTarget.Contains(Path.GetExtension(file.Name)))
                        .ToList();
                    _logger.LogInformation($"{collection.Name} has {collection.Files.Length} files with {targetFiles.Count} matching the filter.");
                    var exists = _siteInfo.Collections.Contains(collection.Name);
                    if (!exists)
                    {
                        _logger.LogInformation($"{collection.Name} is not a collection, treated as directory");
                        result.AddRange(targetFiles);
                    }
                    else
                    {
                        if (exists && _siteInfo.Collections[collection.Name].Output)
                        {
                            _logger.LogInformation($"{collection.Name} is a collection, processing as collection");
                            targetFiles = targetFiles
                                .Select(x =>
                                {
                                    x.MetaData.Collection = collection.Name;
                                    return x;
                                })
                                .ToList();
                            result.AddRange(targetFiles);
                        }
                        else
                        {
                            _logger.LogInformation($"{collection.Name} is a collection, but output == false");
                        }
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
                var keyName = collection[1..];
                var targetFiles = _fileSystem.GetFiles(collection);
                var files = await ProcessFiles(targetFiles.ToArray(), keyName);

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
            return await ProcessFiles(fileInfos.ToArray(), scope: null);
        }

        private async Task<List<File>> ProcessFiles(IFileInfo[] files, string scope)
        {
            var result = new List<File>();
            foreach (var fileInfo in files)
            {
                var fileStream = fileInfo.CreateReadStream();
                using var streamReader = new StreamReader(fileStream);

                var rawContent = await streamReader.ReadToEndAsync();
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
                    LastModified = /* fileMeta.Modified ?? */ fileMeta.Date ?? fileInfo.LastModified,
                    MetaData = fileMeta,
                    Content = fileContents,
                    Name = Path.GetFileName(fileMeta.Uri)
                });
            }
            return result;
        }
    }
}