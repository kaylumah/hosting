using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Interface;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    class Collection
    {
        public string Name { get;set; }
        public string[] Files { get;set; }
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

        public Task GenerateSite()
        {
            const string layoutDir = "_layouts";
            const string includeDir = "_includes";
            string[] templateDirs = new string[] { layoutDir, includeDir };

            var directoryContents = 
                _fileProvider.GetDirectoryContents("")
                    .Where(fileInfo => !(fileInfo.IsDirectory && templateDirs.Contains(fileInfo.Name)));

            var collections = new List<Collection>();
            var collectionDirectories = directoryContents.Where(x => x.IsDirectory);
            foreach(var collection in collectionDirectories)
            {
                var collectionFiles = GetFiles(collection.Name);
                collections.Add(new Collection { Name = collection.Name, Files = collectionFiles.Select(x => x.PhysicalPath).ToArray() });
            }

            var rootFile = _fileProvider.GetDirectoryContents("").First();
            var root = rootFile.PhysicalPath.Replace(rootFile.Name, "");
            var files = directoryContents.Where(x => !x.IsDirectory).Select(x => x.PhysicalPath).ToList();
            files.AddRange(collections.SelectMany(x => x.Files));
            var relativeFileNames = files.Select(x => x.Replace(root, ""));
            return Task.CompletedTask;
        }

        private List<IFileInfo> GetFiles(string path)
        {
            var result = new List<IFileInfo>();

            var info = _fileProvider.GetDirectoryContents(path);
            var directories = info.Where(x => x.IsDirectory);
            result.AddRange(info.Where(x => !x.IsDirectory));

            foreach(var directory in directories)
            {
                result.AddRange(GetFiles(Path.Combine(path, directory.Name)));
            }

            return result;
        }
    }
}
