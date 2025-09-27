// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.iFX.Data.Csv;
using Kaylumah.Ssg.iFX.Data.Json;
using Kaylumah.Ssg.iFX.Data.Yaml;

namespace Kaylumah.Ssg.Manager.Site.Service
{
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
            object result = _JsonParser.Parse<System.Text.Json.Nodes.JsonNode>(file);
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
