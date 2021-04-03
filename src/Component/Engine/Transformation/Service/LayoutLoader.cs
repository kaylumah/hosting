// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Kaylumah.Ssg.Utilities
{
    public class LayoutMetadata
    {
        [YamlMember(Alias = "layout")]
        public string Layout { get; set; }
    }

    public class LayoutLoader
    {
        private readonly IFileSystem _fileSystem;
        public LayoutLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task<List<File<LayoutMetadata>>> Load(string layoutFolder)
        {
            var result = new List<File<LayoutMetadata>>();
            var templateDirectoryContents = _fileSystem.GetDirectoryContents(layoutFolder);
            foreach (var file in templateDirectoryContents)
            {
                var fileInfo = await _fileSystem.GetFile<LayoutMetadata>(Path.Combine(layoutFolder, file.Name));
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