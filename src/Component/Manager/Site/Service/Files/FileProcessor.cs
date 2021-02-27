using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IEnumerable<IContentPreprocessorStrategy> _preprocessorStrategies;
        private readonly MetadataUtil _metadataUtil;
        private readonly Dictionary<string, string> _extensionMapping = new Dictionary<string, string>()
        {
            { ".md", ".html" }
        };

        private readonly SiteInfo _siteInfo;

        public FileProcessor(IFileSystem fileSystem, ILogger<FileProcessor> logger, IEnumerable<IContentPreprocessorStrategy> preprocessorStrategies, IOptions<SiteInfo> options)
        {
            _siteInfo = options.Value;
            _preprocessorStrategies = preprocessorStrategies;
            _fileSystem = fileSystem;
            _logger = logger;
            _metadataUtil = new MetadataUtil();
        }

        public async Task<IEnumerable<File>> Process(FileFilterCriteria criteria)
        {
            var directoryContents = _fileSystem.GetDirectoryContents(string.Empty);

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

            var result = new List<File>();
            result.AddRange(files);

            var collections = await ProcessDirectories(directoriesToProcessAsCollection.Select(x => x.Name).ToArray());
            foreach(var collection in collections)
            {
                var targetFiles = collection
                    .Files
                    .Where(file => criteria.FileExtensionsToTarget.Contains(Path.GetExtension(file.Name)))
                    .ToList();
                _logger.LogInformation($"{collection.Name} has {collection.Files.Length} files with {targetFiles.Count} matching the filter.");
                var keyName = collection.Name[1..];
                var exists = _siteInfo.Collections.Contains(keyName);
                if (!exists)
                {
                    _logger.LogInformation($"{keyName} is not a collection, treated as directory");
                    result.AddRange(targetFiles);
                }

                if (exists && _siteInfo.Collections[keyName].Output)
                {
                    _logger.LogInformation($"{keyName} is a collection, processing as collection");
                    targetFiles = targetFiles
                        .Select(x => {
                            x.MetaData.Collection = keyName;
                            return x;
                        })
                        .ToList();
                    result.AddRange(targetFiles);
                }
                else
                {
                    _logger.LogInformation($"{keyName} is a collection, but output == false");
                }
            }

            return result;
        }

        private async Task<List<FileCollection>> ProcessDirectories(string[] collections)
        {
            var result = new List<FileCollection>();
            foreach(var collection in collections)
            {
                var targetFiles = _fileSystem.GetFiles(collection);
                var files = await ProcessFiles(targetFiles.ToArray());

                result.Add(new FileCollection {
                    Name = collection,
                    Files = files.ToArray()
                });
            }
            return result;
        }

        private async Task<List<File>> ProcessFiles(string[] files)
        {
            var fileInfos = new List<IFileInfo>();
            foreach(var file in files)
            {
                fileInfos.Add(_fileSystem.GetFile(file));
            }
            return await ProcessFiles(fileInfos.ToArray());
        }

        private async Task<List<File>> ProcessFiles(IFileInfo[] files)
        {
            var result = new List<File>();
            foreach(var fileInfo in files)
            {
                var fileStream = fileInfo.CreateReadStream();
                using var streamReader = new StreamReader(fileStream);

                var rawContent = await streamReader.ReadToEndAsync();
                var response = _metadataUtil.Retrieve<FileMetaData>(rawContent);

                var fileMeta = response.Data;
                var fileContents = response.Content;

                fileMeta = Process(fileMeta);
                DetermineOutputLocation(fileInfo, fileMeta);

                var preprocessor = _preprocessorStrategies.SingleOrDefault(x => x.ShouldExecute(fileInfo));
                if (preprocessor != null)
                {
                    fileContents = preprocessor.Execute(fileContents);
                }

                result.Add(new File {
                    MetaData = fileMeta,
                    Content = fileContents,
                    Name = Path.GetFileName(fileMeta.Uri)
                });
            }
            return result;
        }

        private FileMetaData Process(FileMetaData source)
        {
            var result = new FileMetaData();

            // TODO extend with defaults
            // TODO extend with collection / path defaults
            
            if (source != null)
            {
                foreach(var entry in source)
                {
                    if (result.ContainsKey(entry.Key))
                    {
                        // TODO log that its overwritten...
                    }
                    result[entry.Key] = entry.Value;
                }
            }

            return result;
        }

        private void DetermineOutputLocation(IFileInfo fileInfo, FileMetaData metaData)
        {
            // TODO
            metaData.Permalink = "/:year/:month/:day/:name:ext";

            var pattern = @"((?<year>\d{4})\-(?<month>\d{2})\-(?<day>\d{2})\-)?(?<filename>[\s\S]*?)\.(?<ext>.*)";
            var match = Regex.Match(fileInfo.Name, pattern);

            var outputFileName = match.FileNameByPattern();
            var fileDate = match.DateByPattern();
            if (fileDate != null)
            {
                metaData["date"] = fileDate;
            }
            var outputExtension = RetrieveExtension(outputFileName);

            var result = metaData.Permalink
                .Replace("/:year", fileDate == null ? string.Empty : $"/{fileDate?.ToString("yyyy")}")
                .Replace("/:month", fileDate == null ? string.Empty : $"/{fileDate?.ToString("MM")}")
                .Replace("/:day", fileDate == null ? string.Empty : $"/{fileDate?.ToString("dd")}");

            result = result.Replace(":name", Path.GetFileNameWithoutExtension(outputFileName))
                .Replace(":ext", outputExtension);

            if (result.StartsWith("/"))
            {
                result = result[1..];
            }

            metaData.Uri = result;
            metaData.Remove(nameof(metaData.Permalink).ToLower());
        }

        private string RetrieveExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (_extensionMapping.ContainsKey(ext))
            {
                return _extensionMapping[ext];
            }
            return ext;
        }
    }
}