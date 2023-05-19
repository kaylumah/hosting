// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;
using Kaylumah.Ssg.Utilities;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Engine.Transformation.Service;

public class LayoutLoader
{
    private readonly IFileSystem _fileSystem;
    private readonly IMetadataProvider _metadataProvider;
    public LayoutLoader(IFileSystem fileSystem, IMetadataProvider metadataProvider)
    {
        _fileSystem = fileSystem;
        _metadataProvider = metadataProvider;
    }

    public async Task<List<File<LayoutMetadata>>> Load(string layoutFolder)
    {
        var result = new List<File<LayoutMetadata>>();
        var templateDirectoryContents = _fileSystem.GetFiles(layoutFolder);
        foreach (var file in templateDirectoryContents)
        {
            var path = Path.Combine(layoutFolder, file.Name);
            var fileInfo = _fileSystem.GetFile(path);

            var encoding = fileInfo.CreateReadStream().DetermineEncoding();
            var fileName = fileInfo.Name;
            using var streamReader = new StreamReader(fileInfo.CreateReadStream());

            var text = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            var metadata = _metadataProvider.Retrieve<LayoutMetadata>(text);

            var fileWithMeta = new File<LayoutMetadata>
            {
                Encoding = encoding.WebName,
                Name = fileName,
                Path = path,
                Content = metadata.Content,
                Data = metadata.Data
            };

            result.Add(fileWithMeta);

            //var fileInfo = await _fileSystem.GetFile<LayoutMetadata>(path);
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
        var dependencies = templates.Where(x => x.Data != null && !string.IsNullOrEmpty(x.Data.Layout) && template.Name.Equals(x.Data.Layout, StringComparison.Ordinal));
        foreach (var dependency in dependencies)
        {
            var mergedLayout = template.Content.Replace("{{ content }}", dependency.Content);
            dependency.Content = mergedLayout;
            Merge(dependency, templates);
        }
    }
}
