// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;

namespace Ssg.Extensions.Metadata.YamlFrontMatter
{
    public class YamlFrontMatterMetadataProvider : IMetadataProvider
    {
        readonly string _Pattern;

        readonly IYamlParser _YamlParser;
        public YamlFrontMatterMetadataProvider(IYamlParser yamlParser)
        {
            _YamlParser = yamlParser;
            _Pattern = @"\A(---\s*\n.*?\n?)(?<yaml>[\s\S]*?)(---)";
        }

        public Metadata<T> Retrieve<T>(string contents)
        {
            string frontMatterData = string.Empty;
            Match match = Regex.Match(contents, _Pattern);
            if (match.Success)
            {
                frontMatterData = match.Groups["yaml"].Value.TrimEnd();
                string frontMatter = match.Value;
                contents = contents.Replace(frontMatter, string.Empty).TrimStart();
            }

            T data = _YamlParser.Parse<T>(frontMatterData);
#pragma warning disable IDESIGN103
            Metadata<T> result = new Metadata<T>()
            {
                Content = contents,
                Data = data
            };
#pragma warning restore IDESIGN103
            return result;
        }
    }
}
