using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service
{

    public class File
    {
        public FileMetaData MetaData { get;set; }
        public string Contents { get;set; }
    }

    [DebuggerDisplay("{Name} {Files.Length} Files")]
    public class FileCollection
    {
        public string Name { get;set; }
        public File[] Files { get;set; }
    }

    public class FileMetaData : Dictionary<string, object>
    {
        public string Layout
        {
            get {
                if (ContainsKey(nameof(Layout).ToLower()))
                {
                    return (string) this[nameof(Layout).ToLower()];
                }
                return null;
            }
            set {
                this[nameof(Layout).ToLower()] = value;
            }
        }
    }

    public interface IFileProcessor {
        Task<IEnumerable<File>> Process();
    }

    public class CustomFileProcessor : IFileProcessor
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly MetadataUtil _metadataUtil;
        private readonly string[] _extensions = new string[] { ".md", ".html", ".xml" };
        private readonly Dictionary<string, string> _extensionMapping = new Dictionary<string, string>()
        {
            { ".md", ".html" }
        };

        public CustomFileProcessor(IFileSystem fileSystem, ILogger<CustomFileProcessor> logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _metadataUtil = new MetadataUtil();
        }

        public async Task<IEnumerable<File>> Process()
        {
            var directoryContents = _fileSystem.GetDirectoryContents(string.Empty);

            var directoriesToProcessAsCollection = directoryContents.Where(info => info.IsDirectory);
            var filesWithoutCollections = directoryContents.Where(info => !info.IsDirectory);
            
            var files = 
                await ProcessFiles(
                    filesWithoutCollections
                    .Where(
                        fileInfo => _extensions.Contains(Path.GetExtension(fileInfo.Name))
                    )
                    .Select(x => x.Name)
                    .ToArray()
                );
            var collections = await ProcessCollections(directoriesToProcessAsCollection.Select(x => x.Name).ToArray());

            var result = new List<File>();
            result.AddRange(files);
            result.AddRange(collections.SelectMany(x => x.Files));
            return result;
        }

        private async Task<List<FileCollection>> ProcessCollections(string[] collections)
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
                result.Add(new File {
                    MetaData = response.Data,
                    Contents = response.Content
                });
            }
            return result;
        }
    }
}