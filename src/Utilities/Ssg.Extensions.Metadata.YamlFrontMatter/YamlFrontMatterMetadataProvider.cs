using System.Text.RegularExpressions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ssg.Extensions.Metadata.YamlFrontMatter
{
    public class YamlFrontMatterMetadataProvider : IMetadataProvider
    {
        private const string _pattern = @"\A(---\s*\n.*?\n?)(?<yaml>[\s\S]*?)(---)";
        
        private readonly IYamlParser _yamlParser;
        public YamlFrontMatterMetadataProvider(IYamlParser yamlParser)
        {
            _yamlParser = yamlParser;
        }

        public Metadata<T> Retrieve<T>(string contents)
        {
            var frontMatterData = string.Empty;
            var match = Regex.Match(contents, _pattern);
            if (match.Success)
            {
                frontMatterData = match.Groups["yaml"].Value.TrimEnd();
                var frontMatter = match.Value;
                contents = contents.Replace(frontMatter, string.Empty).TrimStart();
            }

            return new Metadata<T>
            {
                Content = contents,
                Data = _yamlParser.Parse<T>(frontMatterData)
            };
        }
    }
}
