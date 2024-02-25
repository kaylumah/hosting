// Copyright (c) Kaylumah, 2024. All rights reserved.
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

        readonly IFileSystem _FileSystem;
        readonly ILogger _Logger;
        readonly IEnumerable<IContentPreprocessorStrategy> _PreprocessorStrategies;
        readonly IFileParser _FileMetaDataProcessor;
        readonly SiteInfo _SiteInfo;

        public FileProcessor(
            IFileSystem fileSystem,
            ILogger<FileProcessor> logger,
            IEnumerable<IContentPreprocessorStrategy> preprocessorStrategies,
            SiteInfo options,
            IFileParser fileMetadataParser)
        {
            _SiteInfo = options;
            _PreprocessorStrategies = preprocessorStrategies;
            _FileSystem = fileSystem;
            _Logger = logger;
            _FileMetaDataProcessor = fileMetadataParser;
        }

        public async Task<IEnumerable<TextFile>> Process(FileFilterCriteria criteria)
        {
            List<TextFile> result = new List<TextFile>();

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
            List<TextFile> files = await ProcessFiles(fileNames).ConfigureAwait(false);

            result.AddRange(files);

            string[] directories = directoriesToProcessAsCollection.Select(x => x.Name).ToArray();
            List<FileCollection> collections = await ProcessDirectories(criteria, directories).ConfigureAwait(false);
            foreach (FileCollection collection in collections)
            {
                List<TextFile> targetFiles = collection
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

        async Task<List<TextFile>> ProcessFiles(string[] files)
        {
            List<IFileInfo> fileInfos = new List<IFileInfo>();
            foreach (string file in files)
            {
                IFileInfo fileInfo = _FileSystem.GetFile(file);
                fileInfos.Add(fileInfo);
            }

            IFileInfo[] fileInfosArray = fileInfos.ToArray();
            List<TextFile> result = await ProcessFilesInScope(fileInfosArray, scope: null).ConfigureAwait(false);
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
            using System.IDisposable? logScope = _Logger.BeginScope($"[Directory: '{directory}']");
            string keyName = directory[1..];
            string collectionDirectory = Path.Combine(criteria.RootDirectory, directory);
            List<IFileSystemInfo> targetFiles = _FileSystem.GetFiles(collectionDirectory).Where(x => !x.IsDirectory()).ToList();
            IFileSystemInfo[] targetFilesArray = targetFiles.ToArray();
            List<TextFile> files = await ProcessFilesInScope(targetFilesArray, keyName).ConfigureAwait(false);

            FileCollection fileCollection = new FileCollection();
            fileCollection.Name = keyName;
            fileCollection.Files = files.ToArray();
            return fileCollection;
        }

        async Task<List<TextFile>> ProcessFilesInScope(IFileSystemInfo[] files, string? scope)
        {
            List<TextFile> result = new List<TextFile>();
            foreach (IFileSystemInfo fileInfo in files)
            {
                TextFile fileResult = await ProcessFileInScope(fileInfo, scope);
                result.Add(fileResult);
            }

            return result;
        }

        async Task<TextFile> ProcessFileInScope(IFileSystemInfo fileInfo, string? scope)
        {
            using System.IDisposable? logScope = _Logger.BeginScope($"[File: '{fileInfo.Name}']");
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
            ParsedFile<FileMetaData> response = _FileMetaDataProcessor.Parse(criteria);

            FileMetaData fileMeta = response.Data;
            string fileContents = response.Content;

            IContentPreprocessorStrategy? preprocessor = _PreprocessorStrategies.SingleOrDefault(x => x.ShouldExecute(fileInfo));
            if (preprocessor != null)
            {
                fileContents = preprocessor.Execute(fileContents);
            }

            TextFile fileResult = new TextFile(fileMeta, fileContents);
            return fileResult;
        }
    }
}
