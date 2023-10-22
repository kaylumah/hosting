// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;

namespace Ssg.Extensions.Metadata.YamlFrontMatter
{
    public class YamlFrontMatterMetadataProvider : IMetadataProvider
    {
        const string _pattern = @"\A(---\s*\n.*?\n?)(?<yaml>[\s\S]*?)(---)";

        readonly IYamlParser _yamlParser;
        public YamlFrontMatterMetadataProvider(IYamlParser yamlParser)
        {
            _yamlParser = yamlParser;
        }

        public Metadata<T> Retrieve<T>(string contents)
        {
            string frontMatterData = string.Empty;
            Match match = Regex.Match(contents, _pattern);
            if (match.Success)
            {
                frontMatterData = match.Groups["yaml"].Value.TrimEnd();
                string frontMatter = match.Value;
                contents = contents.Replace(frontMatter, string.Empty).TrimStart();
            }

            T data = _yamlParser.Parse<T>(frontMatterData);
            Metadata<T> result = new Metadata<T>()
         
            {
                Content = contents,
                Data = data
            };
            return result;
        }
    }
}
