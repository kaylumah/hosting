using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class LayoutMetadata
    {
        public string Layout { get;set; }
    }
    public class LayoutLoader
    {
        private readonly IFileProvider _fileProvider;
        private readonly FileUtil _fileUtil;
        public LayoutLoader(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
            _fileUtil = new FileUtil(_fileProvider);
        }

        public async Task<List<File<LayoutMetadata>>> Load(string layoutFolder)
        {
            var result = new List<File<LayoutMetadata>>();
            var templateDirectoryContents = _fileProvider.GetDirectoryContents(layoutFolder);
            foreach (var file in templateDirectoryContents)
            {
                var fileInfo = await _fileUtil.GetFileInfo<LayoutMetadata>(Path.Combine(layoutFolder, file.Name));
                result.Add(fileInfo);
            }

            var baseTemplates = result
                .Where(template => template.Data == null)
                .ToList();

            foreach (var template in baseTemplates)
            {
                Merge(template, result);
            }

            return result;
        }

        private void Merge(File<LayoutMetadata> template, List<File<LayoutMetadata>> templates)
        {
            var dependencies = templates.Where(x => x.Data != null && !string.IsNullOrEmpty(x.Data.Layout) && template.Name.Equals(x.Data.Layout));
            foreach (var dependency in dependencies)
            {
                var mergedLayout = template.Content.Replace("{{ content }}", dependency.Content);
                dependency.Content = mergedLayout;
                Merge(dependency, templates);
            }
        }
    }

    public class FileUtil
    {
        private readonly IFileProvider _fileProvider;
        public FileUtil(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
        public async Task<File<T>> GetFileInfo<T>(string relativePath)
        {
            var fileInfo = _fileProvider.GetFileInfo(relativePath);
            var encoding = new EncodingUtil().DetermineEncoding(fileInfo.CreateReadStream());
            var fileName = fileInfo.Name;
            using var streamReader = new StreamReader(fileInfo.CreateReadStream());
            var text = await streamReader.ReadToEndAsync();
            var metadata = new MetadataUtil().Retrieve<T>(text);
            return new File<T>
            {
                Encoding = encoding.WebName,
                Name = fileName,
                Path = relativePath,
                Content = metadata.Content,
                Data = metadata.Data
            };
        }
    }

    public class File<TMetadata>
    {
        public string Encoding { get;set; }
        public string Name { get;set; }
        public string Path { get;set; }
        public string Content { get;set; }
        public TMetadata Data { get;set; }
    }

    class Collection
    {
        public string Name { get; set; }
        public string[] Files { get; set; }
    }

    public class SiteManager : ISiteManager
    {
        private readonly IFileProvider _fileProvider;
        private readonly ILogger _logger;

        public SiteManager(IFileProvider fileProvider, ILogger<SiteManager> logger)
        {
            _fileProvider = fileProvider;
            _logger = logger;
        }
        public async Task GenerateSite()
        {
            const string layoutDir = "_layouts";
            const string includeDir = "_includes";
            string[] templateDirs = new string[] { layoutDir, includeDir };

            var templates = await new LayoutLoader(_fileProvider).Load(layoutDir);

            var directoryContents =
                            _fileProvider.GetDirectoryContents("");

            var rootFile = directoryContents.FirstOrDefault();

            if (rootFile != null)
            {

                var directoryInfos = directoryContents.Where(fileInfo => !(fileInfo.IsDirectory && templateDirs.Contains(fileInfo.Name)));
                var collectionDirectories = directoryInfos.Where(x => x.IsDirectory);
                var files = directoryInfos.Where(x => !x.IsDirectory).Select(x => x.PhysicalPath).ToList();

                var collections = new List<Collection>();
                foreach (var collection in collectionDirectories)
                {
                    var collectionFiles = GetFiles(collection.Name);
                    collections.Add(new Collection { Name = collection.Name, Files = collectionFiles.Select(x => x.PhysicalPath).ToArray() });
                }

                var root = rootFile.PhysicalPath.Replace(rootFile.Name, "");
                files.AddRange(collections.SelectMany(x => x.Files));
                var relativeFileNames = files.Select(x => x.Replace(root, ""));

                // foreach (var filePath in relativeFileNames)
                // {
                //     await GetFileInfo(filePath);
                // }
            }
        }

        private List<IFileInfo> GetFiles(string path)
        {
            var result = new List<IFileInfo>();

            var info = _fileProvider.GetDirectoryContents(path);
            var directories = info.Where(x => x.IsDirectory);
            result.AddRange(info.Where(x => !x.IsDirectory));

            foreach (var directory in directories)
            {
                result.AddRange(GetFiles(Path.Combine(path, directory.Name)));
            }

            return result;
        }
    }
}
