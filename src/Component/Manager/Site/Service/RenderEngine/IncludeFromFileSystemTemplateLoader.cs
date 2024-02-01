// Copyright (c) Kaylumah, 2024. All rights reserved.
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

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    sealed class IncludeFromFileSystemTemplateLoader : ITemplateLoader
    {
        readonly IFileSystem _FileSystem;
        readonly string _TemplateFolder;
        public IncludeFromFileSystemTemplateLoader(IFileSystem fileSystem, string templateFolder)
        {
            _FileSystem = fileSystem;
            _TemplateFolder = templateFolder;
        }

        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            string templateFolderPath = _FileSystem.GetFile(_TemplateFolder).FullName;
            string templateFilePath = _FileSystem.Path.Combine(templateFolderPath, templateName);
            return templateFilePath;
        }

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            // unused...
            IFileInfo fileInfo = _FileSystem.GetFile(templatePath);
            Stream readStream = fileInfo.CreateReadStream();
            using StreamReader reader = new StreamReader(readStream);
            string result = reader.ReadToEnd();
            return result;
        }

        public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            IFileInfo templateFileInfo = _FileSystem.GetFile(templatePath);
            using Stream templateReadStream = templateFileInfo.CreateReadStream();
            using StreamReader templateStreamReader = new StreamReader(templateReadStream);
            string templateContent = await templateStreamReader.ReadToEndAsync().ConfigureAwait(false);

            bool templateIsHtml = ".html".Equals(templateFileInfo.Extension, StringComparison.OrdinalIgnoreCase);
            bool developerMode = IsDeveloperMode();

            bool includeDevelopmentInfo = templateIsHtml && developerMode;
            if (includeDevelopmentInfo)
            {
                StringBuilder sb = new StringBuilder();
                string beginTemplate = string.Format(CultureInfo.InvariantCulture, "<!-- BEGIN Template: '{0}' -->", templatePath);
                sb.AppendLine(beginTemplate);
                sb.Append(templateContent);
                sb.AppendLine();
                string endTemplate = string.Format(CultureInfo.InvariantCulture, "<!-- END Template: '{0}' -->", templatePath);
                sb.AppendLine(endTemplate);
                string modifiedContent = sb.ToString();
                return modifiedContent;
            }

            return templateContent;
        }

        static bool IsDeveloperMode()
        {
            string developerMode = Environment.GetEnvironmentVariable("DEVELOPER_MODE") ?? "false";
            bool succeeded = bool.TryParse(developerMode, out bool developerModeActive);
            bool result = succeeded && developerModeActive;
            return result;
        }
    }
}
