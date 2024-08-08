// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
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

        public void Execute(FileMetaData fileMetaData)
        {
            string raw = fileMetaData.Raw;
            MarkdownUtil markdownUtil = new MarkdownUtil(_SiteInfo.Url);
            fileMetaData.Content = markdownUtil.ToHtml(raw);
            fileMetaData["PlainText"] = markdownUtil.ToText(raw);
        }

        public bool ShouldExecute(FileMetaData fileMetaData)
        {
            string extension = Path.GetExtension(fileMetaData.SourceFileName);
            bool isMatch = _TargetExtensions.Contains(extension);
            return isMatch;
        }
    }
}
