// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;

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