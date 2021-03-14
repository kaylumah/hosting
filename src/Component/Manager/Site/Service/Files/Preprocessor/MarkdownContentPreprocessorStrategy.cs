using System.IO;
using System.Linq;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class MarkdownContentPreprocessorStrategy : IContentPreprocessorStrategy
    {
        private readonly string[] _targetExtensions = new string[] { ".md" };
        private readonly MarkdownUtil _markdownUtil = new MarkdownUtil();

        public string Execute(string raw)
        {
            return _markdownUtil.Transform(raw);
        }

        public bool ShouldExecute(IFileInfo fileInfo)
        {
            return _targetExtensions.Contains(Path.GetExtension(fileInfo.Name));
        }
    }
}