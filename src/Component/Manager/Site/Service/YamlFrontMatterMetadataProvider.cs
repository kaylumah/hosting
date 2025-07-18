﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.iFX.Data.Yaml;

namespace Kaylumah.Ssg.Extensions.Metadata.YamlFrontMatter
{
    public partial class YamlFrontMatterMetadataProvider : IFrontMatterMetadataProvider
    {
        [GeneratedRegex(@"\A(---\s*\n.*?\n?)(?<yaml>[\s\S]*?)(---)")]
        private static partial Regex SplitContentFromFrontMatter();

        readonly IYamlParser _YamlParser;
        public YamlFrontMatterMetadataProvider(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
        }

        public ParsedFile<T> Retrieve<T>(string contents)
        {
            string frontMatterData = string.Empty;
            Match match = SplitContentFromFrontMatter().Match(contents);
            if (match.Success)
            {
                frontMatterData = match.Groups["yaml"].Value.TrimEnd();
                string frontMatter = match.Value;
                contents = contents.Replace(frontMatter, string.Empty).TrimStart();
            }

            T data = default(T)!;
            if (!string.IsNullOrEmpty(frontMatterData))
            {
                data = _YamlParser.Parse<T>(frontMatterData);
            }

            ParsedFile<T> result = new ParsedFile<T>(contents, data);
            return result;
        }
    }
}
