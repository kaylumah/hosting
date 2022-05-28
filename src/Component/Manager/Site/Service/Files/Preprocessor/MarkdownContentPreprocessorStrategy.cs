// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;
using Kaylumah.Ssg.Utilities;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;

public class MarkdownContentPreprocessorStrategy : IContentPreprocessorStrategy
{
    private readonly string[] _targetExtensions = new string[] { ".md" };

    public string Execute(string raw)
    {
        return MarkdownUtil.Transform(raw);
    }

    public bool ShouldExecute(IFileSystemInfo fileInfo)
    {
        return _targetExtensions.Contains(Path.GetExtension(fileInfo.Name));
    }
}
