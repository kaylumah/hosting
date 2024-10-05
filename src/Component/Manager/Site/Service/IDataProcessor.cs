// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Data.Csv;
using Ssg.Extensions.Data.Json;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class DataProcessor
    {
        readonly SiteInfo _SiteInfo;
        readonly IFileSystem _FileSystem;
        readonly ILogger _Logger;
        readonly IYamlParser _YamlParser;
        readonly IEnumerable<IKnownFileProcessor> _KnownFileProcessors;
        readonly IEnumerable<IKnownExtensionProcessor> _KnownExtensionProcessors;
        public DataProcessor(SiteInfo siteInfo, IFileSystem fileSystem, ILogger<DataProcessor> logger, IYamlParser yamlParser, IEnumerable<IKnownFileProcessor> knownFileProcessors, IEnumerable<IKnownExtensionProcessor> knownExtensionProcessors)
        {
            _SiteInfo = siteInfo;
            _FileSystem = fileSystem;
            _Logger = logger;
            _YamlParser = yamlParser;
            _KnownFileProcessors = knownFileProcessors;
            _KnownExtensionProcessors = knownExtensionProcessors;
        }

        public void EnrichSiteWithData(SiteMetaData site)
        {
            string dataDirectory = Constants.Directories.SourceDataDirectory;
            string[] extensions = _SiteInfo.SupportedDataFileExtensions.ToArray();
            List<IFileSystemInfo> dataFiles = _FileSystem.GetFiles(dataDirectory)
                .Where(file => !file.IsDirectory())
                .Where(file =>
                {
                    string extension = Path.GetExtension(file.Name);
                    bool result = extensions.Contains(extension);
                    return result;
                })
                .ToList();

            List<string> knownFileNames = _KnownFileProcessors.Select(x => x.KnownFileName).ToList();
            List<IFileSystemInfo> knownFiles = dataFiles.Where(file => knownFileNames.Contains(file.Name)).ToList();
            dataFiles = dataFiles.Except(knownFiles).ToList();

            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (IFileSystemInfo fileSystemInfo in knownFiles)
            {
                IKnownFileProcessor? strategy = _KnownFileProcessors.SingleOrDefault(processor => processor.IsApplicable(fileSystemInfo));
                strategy?.Execute(data, fileSystemInfo);
            }

            foreach (IFileSystemInfo fileSystemInfo in dataFiles)
            {
                IKnownExtensionProcessor? strategy = _KnownExtensionProcessors.SingleOrDefault(processor => processor.IsApplicable(fileSystemInfo));
                strategy?.Execute(data, fileSystemInfo);
            }
        }
    }

    public interface IDataFileProcessor
    {
        void Execute(Dictionary<string, object> data, IFileSystemInfo file);
    }

    public interface IKnownFileProcessor : IDataFileProcessor
    {
        string KnownFileName
        { get; }

        bool IsApplicable(IFileSystemInfo file)
        {
            string fileName = file.Name;
            bool fileNameMatches = fileName.Equals(KnownFileName, StringComparison.Ordinal);
            return fileNameMatches;
        }
    }

    public interface IKnownExtensionProcessor : IDataFileProcessor
    {
        string KnownExtension
        { get; }

        bool IsApplicable(IFileSystemInfo file)
        {
            string extension = file.Extension;
            bool extensionMatches = extension.Equals(KnownExtension, StringComparison.Ordinal);
            return extensionMatches;
        }
    }

    public class CsvFileProcessor : IKnownExtensionProcessor
    {
        readonly ICsvParser _CsvParser;

        public CsvFileProcessor(ICsvParser csvParser)
        {
            _CsvParser = csvParser;
        }

        public string KnownExtension => ".csv";

        public void Execute(Dictionary<string, object> data, IFileSystemInfo file)
        {
            object result = _CsvParser.Parse<object>(file);
            // object result = _CsvParser.Parse<Dictionary<string, object>>(file);
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            data[fileName] = result;
        }
    }

    public class JsonFileProcessor : IKnownExtensionProcessor
    {
        readonly IJsonParser _JsonParser;

        public JsonFileProcessor(IJsonParser jsonParser)
        {
            _JsonParser = jsonParser;
        }

        public string KnownExtension => ".json";

        public void Execute(Dictionary<string, object> data, IFileSystemInfo file)
        {
            object result = _JsonParser.Parse<JsonNode>(file);
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            data[fileName] = result;
        }
    }

    public class YamlFileProcessor : IKnownExtensionProcessor
    {
        readonly IYamlParser _YamlParser;

        public YamlFileProcessor(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
        }

        public string KnownExtension => ".yml";

        public void Execute(Dictionary<string, object> data, IFileSystemInfo file)
        {
            object result = _YamlParser.Parse<object>(file);
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            data[fileName] = result;
        }
    }

    public partial class TagFileProcessor : IKnownFileProcessor
    {
        readonly IYamlParser _YamlParser;

        public string KnownFileName => Constants.KnownFiles.Tags;

        public TagFileProcessor(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
        }

        public void Execute(Dictionary<string, object> data, IFileSystemInfo file)
        {
            TagMetaDataCollection tagData = _YamlParser.Parse<TagMetaDataCollection>(file);
            data["tags"] = tagData;
        }
    }

    public class OrganizationFileProcessor : IKnownFileProcessor
    {
        readonly IYamlParser _YamlParser;

        public OrganizationFileProcessor(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
        }

        public string KnownFileName => Constants.KnownFiles.Organizations;

        public void Execute(Dictionary<string, object> data, IFileSystemInfo file)
        {
            OrganizationMetaDataCollection organizationData = _YamlParser.Parse<OrganizationMetaDataCollection>(file);
            data["organizations"] = organizationData;
        }
    }

    public class AuthorFileProcessor : IKnownFileProcessor
    {
        readonly IYamlParser _YamlParser;

        public AuthorFileProcessor(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
        }

        public string KnownFileName => Constants.KnownFiles.Authors;

        public void Execute(Dictionary<string, object> data, IFileSystemInfo file)
        {
            AuthorMetaDataCollection authorData = _YamlParser.Parse<AuthorMetaDataCollection>(file);
            data["authors"] = authorData;
        }
    }
}
