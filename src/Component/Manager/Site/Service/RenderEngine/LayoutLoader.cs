// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaylumah.Ssg.Utilities;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public class LayoutLoader
    {
        readonly IFileSystem _FileSystem;
        readonly IFrontMatterMetadataProvider _MetadataProvider;
        public LayoutLoader(IFileSystem fileSystem, IFrontMatterMetadataProvider metadataProvider)
        {
            _FileSystem = fileSystem;
            _MetadataProvider = metadataProvider;
        }

        public async Task<List<File<LayoutMetadata>>> Load(string layoutFolder)
        {
            List<File<LayoutMetadata>> result = new List<File<LayoutMetadata>>();
            IEnumerable<IFileSystemInfo> templateDirectoryContents = _FileSystem.GetFiles(layoutFolder);
            foreach (IFileSystemInfo file in templateDirectoryContents)
            {
                string path = Path.Combine(layoutFolder, file.Name);
                IFileInfo fileInfo = _FileSystem.GetFile(path);

                Encoding encoding = fileInfo.CreateReadStream().DetermineEncoding();
                string fileName = fileInfo.Name;
                Stream stream = fileInfo.CreateReadStream();
                using StreamReader streamReader = new StreamReader(stream);

                string text = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                ParsedFile<LayoutMetadata> metadata = _MetadataProvider.Retrieve<LayoutMetadata>(text);
                string content = metadata.Content;

                bool templateIsHtml = ".html".Equals(fileInfo.Extension, StringComparison.OrdinalIgnoreCase);
                bool developerMode = IsDeveloperMode();

                bool includeDevelopmentInfo = templateIsHtml && developerMode;
                if (includeDevelopmentInfo)
                {
                    StringBuilder sb = new StringBuilder();
                    string beginTemplate = string.Format(CultureInfo.InvariantCulture, "<!-- BEGIN Layout: '{0}' -->", path);
                    sb.AppendLine(beginTemplate);
                    sb.Append(content);
                    sb.AppendLine();
                    string endTemplate = string.Format(CultureInfo.InvariantCulture, "<!-- END Layout: '{0}' -->", path);
                    sb.AppendLine(endTemplate);
                    string modifiedContent = sb.ToString();
                    content = modifiedContent;
                }

                File<LayoutMetadata> fileWithMeta = new File<LayoutMetadata>(path, content, metadata.FrontMatter, encoding.WebName);
                result.Add(fileWithMeta);
            }

            List<File<LayoutMetadata>> baseTemplates = result
                .Where(template => template.Data == null)
                .ToList();

            foreach (File<LayoutMetadata> template in baseTemplates)
            {
                Merge(template, result);
            }

            return result;
        }

        void Merge(File<LayoutMetadata> template, List<File<LayoutMetadata>> templates)
        {
            IEnumerable<File<LayoutMetadata>> dependencies = templates.Where(x => x.Data != null && !string.IsNullOrEmpty(x.Data.Layout) && template.Name.Equals(x.Data.Layout, StringComparison.Ordinal));
            foreach (File<LayoutMetadata> dependency in dependencies)
            {
                string mergedLayout = template.Content.Replace("{{ content }}", dependency.Content);
                dependency.Content = mergedLayout;
                Merge(dependency, templates);
            }
        }

        static bool IsDeveloperMode()
        {
            string developerMode = Environment.GetEnvironmentVariable("DEVELOPER_MODE") ?? "false";
            bool succeeded = bool.TryParse(developerMode, out bool developerModeActive);
            bool result = succeeded && developerModeActive;
            return result;
        }
    }
}
