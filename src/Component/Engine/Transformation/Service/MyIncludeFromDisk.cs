// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Utilities;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Kaylumah.Ssg.Engine.Transformation.Service;

internal class MyIncludeFromDisk : ITemplateLoader
{
    private readonly IFileSystem _fileSystem;
    private readonly string _templateFolder;
    public MyIncludeFromDisk(IFileSystem fileSystem, string templateFolder)
    {
        _fileSystem = fileSystem;
        _templateFolder = templateFolder;
    }

    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
    {
        var templateLocation = Path.Combine(_fileSystem.GetFile(_templateFolder).FullName, templateName);
        return templateLocation;
        // return Path.Combine(Environment.CurrentDirectory, templateName);
    }

    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        using var reader = new StreamReader(_fileSystem.GetFile(templatePath).CreateReadStream());
        return reader.ReadToEnd();
        //return File.ReadAllText(templatePath);
    }

    public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        using var reader = new StreamReader(_fileSystem.GetFile(templatePath).CreateReadStream());
        return await reader.ReadToEndAsync();
        // return await File.ReadAllTextAsync(templatePath);
    }
}
