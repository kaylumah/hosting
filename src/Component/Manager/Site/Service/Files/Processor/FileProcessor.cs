
// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public partial class FileProcessor : IFileProcessor
    {
        [LoggerMessage(
           EventId = 0,
           Level = LogLevel.Warning,
           Message = "No files present")]
        private partial void LogNoFiles();

        readonly IFileSystem _FileSystem;
        readonly ILogger _Logger;
        readonly IEnumerable<IContentPreprocessorStrategy> _PreprocessorStrategies;
        readonly IFileMetadataParser _FileMetaDataProcessor;
        readonly SiteInfo _SiteInfo;

        public FileProcessor(
            IFileSystem fileSystem,
            ILogger<FileProcessor> logger,
            IEnumerable<IContentPreprocessorStrategy> preprocessorStrategies,
            SiteInfo options,
            IFileMetadataParser fileMetadataParser)
        {
            _SiteInfo = options;
            _PreprocessorStrategies = preprocessorStrategies;
            _FileSystem = fileSystem;
            _Logger = logger;
            _FileMetaDataProcessor = fileMetadataParser;
        }

        public async Task<IEnumerable<File>> Process(FileFilterCriteria criteria)
        {
            List<File> result = new List<File>();

            List<IFileSystemInfo> directoryContents = _FileSystem.GetFiles(criteria.RootDirectory).ToList();

            if (!directoryContents.Any())
            {
                LogNoFiles();
                return result;
            }

            List<IFileSystemInfo> directoriesToProcessAsCollection = directoryContents
                .Where(info => info.IsDirectory() && !criteria.DirectoriesToSkip.Contains(info.Name))
                .ToList();

            List<IFileSystemInfo> filesWithoutCollections = directoryContents.Where(info =>
                !info.IsDirectory() && criteria.FileExtensionsToTarget.Contains(Path.GetExtension(info.Name))
            ).ToList();

            List<File> files =
                await ProcessFiles(
                    filesWithoutCollections
                    .Select(x => x.FullName)
                    .ToArray()
                ).ConfigureAwait(false);

            result.AddRange(files);

            List<FileCollection> collections = await ProcessDirectories(criteria, directoriesToProcessAsCollection.Select(x => x.Name).ToArray()).ConfigureAwait(false);
            foreach (FileCollection collection in collections)
            {
                List<File> targetFiles = collection
                    .Files
                    .Where(file => criteria.FileExtensionsToTarget.Contains(Path.GetExtension(file.Name)))
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

        async Task<List<FileCollection>> ProcessDirectories(FileFilterCriteria criteria, string[] collections)
        {
            List<FileCollection> result = new List<FileCollection>();
            foreach (string collection in collections)
            {
                using System.IDisposable logScope = _Logger.BeginScope($"[ProcessDirectories '{collection}']");
                string keyName = collection[1..];
                List<IFileSystemInfo> targetFiles = _FileSystem.GetFiles(Path.Combine(criteria.RootDirectory, collection)).Where(x => !x.IsDirectory()).ToList();
                List<File> files = await ProcessFiles(targetFiles.ToArray(), keyName).ConfigureAwait(false);

                result.Add(new FileCollection
                {
                    Name = keyName,
                    Files = files.ToArray()
                });
            }

            return result;
        }

        async Task<List<File>> ProcessFiles(string[] files)
        {
            List<IFileInfo> fileInfos = new List<IFileInfo>();
            foreach (string file in files)
            {
                fileInfos.Add(_FileSystem.GetFile(file));
            }

            return await ProcessFiles(fileInfos.ToArray(), scope: null).ConfigureAwait(false);
        }

        async Task<List<File>> ProcessFiles(IFileSystemInfo[] files, string scope)
        {
            List<File> result = new List<File>();
            foreach (IFileSystemInfo fileInfo in files)
            {
                using System.IDisposable logScope = _Logger.BeginScope($"[ProcessFiles '{fileInfo.Name}']");
                Stream fileStream = fileInfo.CreateReadStream();
                using StreamReader streamReader = new StreamReader(fileStream);

                string rawContent = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                global::Ssg.Extensions.Metadata.Abstractions.Metadata<FileMetaData> response = _FileMetaDataProcessor.Parse(new MetadataCriteria
                {
                    Content = rawContent,
                    Scope = scope,
                    FileName = fileInfo.Name
                });

                FileMetaData fileMeta = response.Data;
                string fileContents = response.Content;

                IContentPreprocessorStrategy preprocessor = _PreprocessorStrategies.SingleOrDefault(x => x.ShouldExecute(fileInfo));
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
}
