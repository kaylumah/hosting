using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service
{

    public class FileProcessor
    {
        private readonly MetadataUtil _metadataUtil;
        private readonly IFileProvider _fileProvider;
        public FileProcessor(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
            _metadataUtil = new MetadataUtil();
        }

        public ContentFile[] Process(string[] targetFiles)
        {
            var result = new List<ContentFile>();

            foreach(var file in targetFiles)
            {
                var fileInfo = _fileProvider.GetFileInfo(file);
                var fileStream = fileInfo.CreateReadStream();
                using var streamReader = new StreamReader(fileStream);
                var rawContent = streamReader.ReadToEnd();

                var originalExtension = Path.GetExtension(file);
                var outputExtension = DetermineTargetExtension(originalExtension);

                var metadata = _metadataUtil.Retrieve<Dictionary<string, object>>(rawContent);
                var layout = metadata.Data.GetValueOrDefault("layout");
                var contentFile = new ContentFile
                {                 
                    Content = metadata.Content,
                    FileName = file
                };
                if (layout != null)
                {
                    contentFile.Layout = (string)layout;
                }
                result.Add(contentFile);
            }

            return result.ToArray();
        }

        private string DetermineTargetExtension(string sourceExtension)
        {
            var mapping = new Dictionary<string, string> {
                { ".md", ".html" }
            };
            return mapping.ContainsKey(sourceExtension) ? mapping[sourceExtension] : sourceExtension;
        }

        /*
        
         private List<ContentFile> ProcessContentFiles(IEnumerable<string> files)
        {
            var result = new List<ContentFile>();
            foreach (var file in files)
            {
                var fileInfo = _fileProvider.GetFileInfo(file);
                

                
                var fileNameWithout = Path.GetFileNameWithoutExtension(file);

                // permalink
                var outputPath = $"{fileNameWithout}{outputExtension}";


            }
            return result;
            // var strategies = new List<IContentStrategy>();
            // var defaultStrategy = new DefaultStrategy();
            // var markdownStrategy = new MarkdownStrategy();
            // strategies.Add(markdownStrategy);
            // foreach(var file in files)
            // {
            //     var fileInfo = _fileProvider.GetFileInfo(file);
                
            //     var strategy = strategies.FirstOrDefault(x => x.ShouldExecute(fileInfo)) ?? defaultStrategy;
            //     strategy.Execute(fileInfo);
            // }
        }
        */
    }


    public class ContentFile
    {
        public string FileName { get;set;}
        public string Layout { get;set; }
        public string Content { get;set; }
        public ContentFile()
        {
        }
    }

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
        private readonly IArtifactAccess _artifactAccess;
        private readonly IFileProvider _fileProvider;
        private readonly ILogger _logger;

        public SiteManager(IArtifactAccess artifactAccess, IFileProvider fileProvider, ILogger<SiteManager> logger)
        {
            _artifactAccess = artifactAccess;
            _fileProvider = fileProvider;
            _logger = logger;
        }

        private BuildData GetBuildData()
        {
            var info = new AssemblyUtil().RetrieveAssemblyInfo(Assembly.GetExecutingAssembly());

            var version = "1.0.0+LOCALBUILD";
            if (info.Version.Length > 6)
            {
                version = info.Version;
            }
            var gitHash = version[(version.IndexOf('+') + 1)..]; // version.Substring(version.IndexOf('+') + 1);
            var shortGitHash = gitHash.Substring(0, 7);
            var repositoryType = info.Metadata["RepositoryType"];
            var repositoryUrl = info.Metadata["RepositoryUrl"];
            var sourceBaseUrl = repositoryUrl.Replace($".{repositoryType}", "/commit");

            return new BuildData();
        }

        private Dictionary<string, object> ParseData(string dataDirectory)
        {
            var extensions = new string[] { ".yml" };
            var dataFiles = GetFiles(dataDirectory)
                .Where(file => extensions.Contains(Path.GetExtension(file.Name)))
                .ToList();
            var data = new Dictionary<string, object>();
            foreach(var file in dataFiles)
            {
                var stream = file.CreateReadStream();
                using var reader = new StreamReader(stream);
                var raw = reader.ReadToEnd();
                var result = new YamlParser().Parse<object>(raw);
                data[Path.GetFileNameWithoutExtension(file.Name)] = result;
            }
            return data;
        }

        public async Task GenerateSite()
        {
            const string layoutDir = "_layouts";
            const string includeDir = "_includes";
            const string dataDir = "_data";
            string[] templateDirs = new string[] { layoutDir, includeDir, dataDir };

            var templates = await new LayoutLoader(_fileProvider).Load(layoutDir);

            var directoryContents =
                            _fileProvider.GetDirectoryContents("");

            var rootFile = directoryContents.FirstOrDefault();

            if (rootFile != null)
            {
                var buildInfo = GetBuildData();
                var siteInfo = new SiteData
                {
                    Data = ParseData(dataDir)
                };






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

                var extensions = new string[] { ".md", ".html", ".xml" };
                var staticFiles = relativeFileNames.Where(fileName => !extensions.Contains(Path.GetExtension(fileName)));
                var contentFiles = new FileProcessor(_fileProvider).Process(
                    relativeFileNames
                    .Where(fileName => extensions.Contains(Path.GetExtension(fileName)))
                    .ToArray()
                );
                var renderRequests = new List<RenderRequest>();
                foreach(var contentFile in contentFiles)
                {
                    renderRequests.Add(new RenderRequest
                    {
                        TemplateName = contentFile.Layout,
                        Model = new RenderData {
                            Build = buildInfo,
                            Site = siteInfo,
                            Content = contentFile.Content
                        }
                    });
                }

                var liquidUtil = new LiquidUtil(_fileProvider);
                var renderResults = await liquidUtil.Render(renderRequests.ToArray());
                var outputDirectory = "dist";
                var artifacts = contentFiles.Select((t, i) => {
                    var renderResult = renderResults[i];
                    return new Artifact {
                        Path = $"{outputDirectory}/{t.FileName}",
                        Contents = Encoding.UTF8.GetBytes(renderResult.Content)
                    };
                }).ToList();
                artifacts.AddRange(staticFiles.Select(staticFile => {
                    return new Artifact {
                        Path = $"{outputDirectory}/{staticFile}",
                        Contents = FileToByteArray(staticFile)
                    };
                }));
                await _artifactAccess.Store(new StoreArtifactsRequest {
                    Artifacts = artifacts.ToArray()
                });
            }
        }

        private byte[] FileToByteArray(string fileName)
        {
            var fileInfo = _fileProvider.GetFileInfo(fileName);
            var fileStream = fileInfo.CreateReadStream();
            return ToByteArray(fileStream);
        }

        private byte[] ToByteArray(Stream input)
        {
            using MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
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
