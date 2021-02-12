using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Manager.Site.Service
{
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
}