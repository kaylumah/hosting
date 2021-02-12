using System.Text.RegularExpressions;

namespace Kaylumah.Ssg.Utilities
{

    public class Metadata<T>
    {
        public T Data { get;set; }
        public string Content { get;set;}
    }

    public class MetadataUtil
    {
        private const string _pattern = @"\A(---\s*\n.*?\n?)(?<yaml>[\s\S]*?)(---)";
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
            var yamlParser = new YamlParser();
            return new Metadata<T>
            {
                Content = contents,
                Data = yamlParser.Parse<T>(frontMatterData)
            };
        }
    }
}