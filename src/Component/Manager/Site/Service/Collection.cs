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

    public class FileProcessor
    {
        private readonly MetadataUtil _metadataUtil;
        private readonly IFileSystem _fileSystem;
        public FileProcessor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _metadataUtil = new MetadataUtil();
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
}