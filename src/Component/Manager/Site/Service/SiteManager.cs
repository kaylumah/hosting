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

    public interface IContentStrategy
    {
        bool ShouldExecute(IFileInfo file);
        void Execute(IFileInfo file);
    }

    public class DefaultStrategy : IContentStrategy
    {
        public void Execute(IFileInfo file)
        {
            // throw new NotImplementedException();
        }

        public bool ShouldExecute(IFileInfo file)
        {
            return false;
        }
    }

    public class MarkdownStrategy : IContentStrategy
    {
        public void Execute(IFileInfo file)
        {
            var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            var renderedContent = new MarkdownUtil().Transform(text);
        }

        public bool ShouldExecute(IFileInfo file)
        {
            // Could be SourceExtension and TargetExtension
            // ie ".md" to ".html"
            return Path.GetExtension(file.Name).Equals(".md");
        }
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

                var extensions = new string[] { ".md" };

                var contentFiles = relativeFileNames.Where(fileName => extensions.Contains(Path.GetExtension(fileName)));
                var staticFiles = relativeFileNames.Where(fileName => !extensions.Contains(Path.GetExtension(fileName)));

                Process(contentFiles);
                // foreach (var filePath in relativeFileNames)
                // {
                //     await GetFileInfo(filePath);
                // }
            }
        }

        private void Process(IEnumerable<string> files)
        {
            var strategies = new List<IContentStrategy>();
            var defaultStrategy = new DefaultStrategy();
            var markdownStrategy = new MarkdownStrategy();
            strategies.Add(markdownStrategy);
            foreach(var file in files)
            {
                var fileInfo = _fileProvider.GetFileInfo(file);
                
                var strategy = strategies.FirstOrDefault(x => x.ShouldExecute(fileInfo)) ?? defaultStrategy;
                strategy.Execute(fileInfo);
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
