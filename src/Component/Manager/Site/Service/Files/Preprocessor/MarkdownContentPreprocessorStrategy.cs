// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor
{
    public class MarkdownContentPreprocessorStrategy : IContentPreprocessorStrategy
    {
        readonly string[] _TargetExtensions;
        readonly SiteInfo _SiteInfo;

        public MarkdownContentPreprocessorStrategy(SiteInfo siteInfo)
        {
            _TargetExtensions = [".md"];
            _SiteInfo = siteInfo;
        }

        public string Execute(string raw)
        {
            MarkdownUtil markdownUtil = new MarkdownUtil(_SiteInfo.Url);
            string result = markdownUtil.ToHtml(raw);
            return result;
        }

        public bool ShouldExecute(IFileSystemInfo fileInfo)
        {
            string extension = Path.GetExtension(fileInfo.Name);
            bool isMatch = _TargetExtensions.Contains(extension);
            return isMatch;
        }
    }
}
