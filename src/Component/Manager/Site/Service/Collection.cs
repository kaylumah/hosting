using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class OldFileProcessor
    {
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