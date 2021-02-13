using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service
{

    class ContentFile
    {
        public string Layout { get;set; }
        public string Content { get;set; }
        public ContentFile(Metadata<Dictionary<string, object>> file)
        {
            Layout = (string) file.Data["layout"];
            Content = file.Content;
        }
    }

    class RenderData
    {
        public string Content { get;set; }
        public SiteData Site { get;set; } = new SiteData();
        public PageData Page { get;set; } = new PageData();
    }

    class SiteData
    {
        public string Title { get;set; } = $"{nameof(SiteData)}{nameof(Title)}";
    }
    class PageData
    {
        public string Title { get;set; } = $"{nameof(PageData)}{nameof(Title)}";
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

                var extensions = new string[] { ".md", ".html" };
                var staticFiles = relativeFileNames.Where(fileName => !extensions.Contains(Path.GetExtension(fileName)));
                var contentFiles = ProcessContentFiles(relativeFileNames
                    .Where(fileName => extensions.Contains(Path.GetExtension(fileName)) )
                );
                var renderRequests = new List<RenderRequest>();
                foreach(var contentFile in contentFiles)
                {
                    renderRequests.Add(new RenderRequest
                    {
                        TemplateName = contentFile.Layout,
                        Model = new RenderData {
                            Content = contentFile.Content
                        }
                    });
                }

                var liquidUtil = new LiquidUtil(_fileProvider);
                var renderResults = await liquidUtil.Render(renderRequests.ToArray());

                var artifacts = contentFiles.Select((t, i) => {
                    var renderResult = renderResults[i];
                    return new Artifact {};
                }).ToList();
                artifacts.AddRange(staticFiles.Select(x => new Artifact {}));
                await _artifactAccess.Store(new StoreArtifactsRequest {
                    Artifacts = artifacts.ToArray()
                });
            }
        }

        private List<ContentFile> ProcessContentFiles(IEnumerable<string> files)
        {
            var result = new List<ContentFile>();
            foreach (var file in files)
            {
                var fileInfo = _fileProvider.GetFileInfo(file);
                var fileStream = fileInfo.CreateReadStream();
                using var streamReader = new StreamReader(fileStream);
                var rawContent = streamReader.ReadToEnd();
                var metadata = new MetadataUtil().Retrieve<Dictionary<string, object>>(rawContent);
                result.Add(new ContentFile(metadata));
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
