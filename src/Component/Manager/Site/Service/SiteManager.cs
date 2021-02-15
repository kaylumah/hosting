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
        private readonly IFileSystem _fileSystem;
        private readonly IFileProvider _fileProvider;
        private readonly ILogger _logger;

        public SiteManager(IArtifactAccess artifactAccess,
            IFileSystem fileSystem,
            IFileProvider fileProvider,
            ILogger<SiteManager> logger)
        {
            _artifactAccess = artifactAccess;
            _fileSystem = fileSystem;
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

            return new BuildData()
            {
                Copyright = info.Copyright,
                GitHash = gitHash,
                ShortGitHash = shortGitHash,
                SourceBaseUri = sourceBaseUrl
            };
        }

        private Dictionary<string, object> ParseData(string dataDirectory)
        {
            var extensions = new string[] { ".yml" };
            var dataFiles = _fileSystem.GetFiles(dataDirectory)
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
            const string assetDir = "assets";
            string[] templateDirs = new string[] { layoutDir, includeDir, dataDir, assetDir };

            var templates = await new LayoutLoader(_fileSystem).Load(layoutDir);

            var directoryContents =
                            _fileProvider.GetDirectoryContents("");

            var rootFile = directoryContents.FirstOrDefault();

            if (rootFile != null)
            {
                var buildInfo = GetBuildData();
                var siteInfo = new SiteData
                {
                    Data = ParseData(dataDir),
                    Collections = new Dictionary<string, object> {
                        { 
                            "pages",
                            new object[] {
                                new {
                                    Uri = "https://kaylumah.nl",
                                    Image = "https://images.unsplash.com/photo-1496128858413-b36217c2ce36?ixlib=rb-1.2.1&ixqx=ek9gmnUEHF&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1679&q=80", 
                                    Type = "Article",
                                    Title = "Boost your conversion rate",
                                    Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Architecto accusantium praesentium eius, ut atque fuga culpa, similique sequi cum eos quis dolorum."
                                },
                                new {
                                    Uri = "https://kaylumah.nl",
                                    Image = "https://images.unsplash.com/photo-1547586696-ea22b4d4235d?ixlib=rb-1.2.1&ixqx=ek9gmnUEHF&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1679&q=80",
                                    Type = "Video",
                                    Title = "How to use search engine optimization to drive sales",
                                    Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Velit facilis asperiores porro quaerat doloribus, eveniet dolore. Adipisci tempora aut inventore optio animi., tempore temporibus quo laudantium."
                                },
                                new {
                                    Uri = "https://kaylumah.nl",
                                    Image = "https://images.unsplash.com/photo-1492724441997-5dc865305da7?ixlib=rb-1.2.1&ixqx=ek9gmnUEHF&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1679&q=80",
                                    Type = "Case Study",
                                    Title = "Improve your customer experience",
                                    Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Sint harum rerum voluptatem quo recusandae magni placeat saepe molestiae, sed excepturi cumque corporis perferendis hic."
                                }
                            }
                        }
                    }
                };

                var directoryInfos = directoryContents.Where(fileInfo => !(fileInfo.IsDirectory && templateDirs.Contains(fileInfo.Name)));
                var collectionDirectories = directoryInfos.Where(x => x.IsDirectory).Select(x => x.Name).ToArray();
                new CollectionLoader(_fileSystem).ProcessCollection(collectionDirectories);

                var files = directoryInfos.Where(x => !x.IsDirectory).Select(x => x.PhysicalPath).ToList();
                var root = rootFile.PhysicalPath.Replace(rootFile.Name, "");
                // files.AddRange(collections.SelectMany(x => x.Files));
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

                var liquidUtil = new LiquidUtil(_fileProvider, _fileSystem);
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
    }
}
