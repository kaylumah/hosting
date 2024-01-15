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
        readonly string[] _TargetExtensions = new string[] { ".md" };

        public string Execute(string raw)
        {
            return MarkdownUtil.Transform(raw);
        }

        public bool ShouldExecute(IFileSystemInfo fileInfo)
        {
            return _TargetExtensions.Contains(Path.GetExtension(fileInfo.Name));
        }
    }
}
