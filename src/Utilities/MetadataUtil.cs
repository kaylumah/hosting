using System.Text.RegularExpressions;

namespace Kaylumah.Ssg.Utilities
{
    public class MetadataUtil
    {
        private const string _pattern = @"\A(---\s*\n.*?\n?)(?<yaml>[\s\S]*?)(---)";
        public object Retrieve(string contents)
        {
            var frontMatterData = string.Empty;
            var match = Regex.Match(contents, _pattern);
            if (match.Success)
            {
                frontMatterData = match.Groups["yaml"].Value.TrimEnd();
                var frontMatter = match.Value;
                contents = contents.Replace(frontMatter, string.Empty).TrimStart();
            }

            return new
            {
                Contents = contents,
                FrontMatter = frontMatterData
            };
        }
    }
}