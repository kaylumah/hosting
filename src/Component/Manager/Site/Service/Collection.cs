using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class FileData
    {
        public string Layout { get;set; }
        public string Title { get;set; }
        public string Description { get;set; }
        public string Uri { get;set; }
        public string Image { get;set; }
        public string Type { get;set; }
    }

    public class ContentFile
    {
        public string FileName { get; set; }
        public string Layout { get; set; }
        public string Content { get; set; }
        public ContentFile()
        {
        }
    }

    public class FileProcessor
    {
        private readonly MetadataUtil _metadataUtil;
        private readonly IFileSystem _fileSystem;
        public FileProcessor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _metadataUtil = new MetadataUtil();
        }

        public ContentFile[] Process(string[] targetFiles)
        {
            var result = new List<ContentFile>();

            foreach (var file in targetFiles)
            {
                var fileInfo = _fileSystem.GetFile(file);
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

    public class CollectionLoader
    {
        private readonly IFileSystem _fileSystem;
        public CollectionLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void ProcessCollection(string[] collectionDirectories)
        {
            var collections = new List<Collection>();
            foreach (var collection in collectionDirectories)
            {
                var collectionFilePath = _fileSystem.GetFile(collection).PhysicalPath;
                var collectionFiles = _fileSystem.GetFiles(collection);
                collections.Add(new Collection { Name = collection, Files = collectionFiles.Select(x => x.PhysicalPath).ToArray() });
            }
        }
    }
}