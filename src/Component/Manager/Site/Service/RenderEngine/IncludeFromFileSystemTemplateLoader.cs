// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Kaylumah.Ssg.Utilities;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine;

sealed class IncludeFromFileSystemTemplateLoader : ITemplateLoader
{
    private readonly IFileSystem _fileSystem;
    private readonly string _templateFolder;
    public IncludeFromFileSystemTemplateLoader(IFileSystem fileSystem, string templateFolder)
    {
        _fileSystem = fileSystem;
        _templateFolder = templateFolder;
    }

    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
    {
        var templateFolderPath = _fileSystem.GetFile(_templateFolder).FullName;
        var templateFilePath = _fileSystem.Path.Combine(templateFolderPath, templateName);
        return templateFilePath;
    }

    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        // unused...
        using var reader = new StreamReader(_fileSystem.GetFile(templatePath).CreateReadStream());
        return reader.ReadToEnd();
    }

    public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        var templateFileInfo = _fileSystem.GetFile(templatePath);
        using var templateReadStream = templateFileInfo.CreateReadStream();
        using var templateStreamReader = new StreamReader(templateReadStream);
        var templateContent = await templateStreamReader.ReadToEndAsync().ConfigureAwait(false);
        return templateContent;
    }
}
