// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Text;
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
        string templateFolderPath = _fileSystem.GetFile(_templateFolder).FullName;
        string templateFilePath = _fileSystem.Path.Combine(templateFolderPath, templateName);
        return templateFilePath;
    }

    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        // unused...
        using StreamReader reader = new StreamReader(_fileSystem.GetFile(templatePath).CreateReadStream());
        return reader.ReadToEnd();
    }

    public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        IFileInfo templateFileInfo = _fileSystem.GetFile(templatePath);
        using Stream templateReadStream = templateFileInfo.CreateReadStream();
        using StreamReader templateStreamReader = new StreamReader(templateReadStream);
        string templateContent = await templateStreamReader.ReadToEndAsync().ConfigureAwait(false);

        bool templateIsHtml = ".html".Equals(templateFileInfo.Extension, System.StringComparison.OrdinalIgnoreCase);
        bool developerMode = IsDeveloperMode();

        bool includeDevelopmentInfo = templateIsHtml && developerMode;
        if (includeDevelopmentInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "<!-- BEGIN Template: '{0}' -->", templatePath));
            sb.Append(templateContent);
            sb.AppendLine();
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "<!-- END Template: '{0}' -->", templatePath));
            string modifiedContent = sb.ToString();
            return modifiedContent;
        }

        return templateContent;
    }

    private static bool IsDeveloperMode()
    {
        string developerMode = Environment.GetEnvironmentVariable("DEVELOPER_MODE") ?? "false";
        bool succeeded = bool.TryParse(developerMode, out bool developerModeActive);
        bool result = succeeded && developerModeActive;
        return result;
    }
}
