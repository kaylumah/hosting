// Copyright (c) Kaylumah, 2024. All rights reserved.
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

        public MarkdownContentPreprocessorStrategy()
        {
            _TargetExtensions = [".md"];
        }

        public string Execute(string raw)
        {
            string result = MarkdownUtil.Transform(raw);
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
