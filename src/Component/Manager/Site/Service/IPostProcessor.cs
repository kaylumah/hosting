// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using HtmlAgilityPack;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface IPostProcessor
    {
        bool IsApplicable(Artifact artifact);

        void Apply(Artifact artifact);
    }

    public abstract class CommonPostProcessor : IPostProcessor
    {
        readonly ILogger _Logger;

        protected CommonPostProcessor(ILogger logger)
        {
            _Logger = logger;
        }

        string GetExtension(Artifact artifact)
        {
            string path = artifact.Path;
            string extension = Path.GetExtension(path);
            return extension;
        }

        protected abstract string GetTargetExtension();

        protected virtual string GetFormattedContent(string content)
        {
            // Default is no transformation
            return content;
        }

        bool IPostProcessor.IsApplicable(Artifact artifact)
        {
            string actual = GetExtension(artifact);
            string target = GetTargetExtension();
            bool result = string.Equals(actual, target, StringComparison.OrdinalIgnoreCase);

            // bool result = false;
            return result;
        }

        void IPostProcessor.Apply(Artifact artifact)
        {
            byte[] originalBytes = artifact.Contents;
            string originalContent = System.Text.Encoding.UTF8.GetString(originalBytes);
            string formattedContent = GetFormattedContent(originalContent);
            bool areContentsEqual = string.Equals(formattedContent, originalContent, StringComparison.OrdinalIgnoreCase);
            if (areContentsEqual == false)
            {
#pragma warning disable CA1848
#pragma warning disable CA2254
                _Logger.LogInformation($"The file '{artifact.Path}' has a change");
#pragma warning restore CA1848
#pragma warning restore CA2254
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(formattedContent);
                artifact.Contents = bytes;
            }
        }
    }

    public class HtmlPostProcessor : CommonPostProcessor
    {
        public HtmlPostProcessor(ILogger<HtmlPostProcessor> logger) : base(logger)
        {
        }

        protected override string GetTargetExtension()
        {
            return ".html";
        }

        protected override string GetFormattedContent(string content)
        {
            HtmlDocument document = new HtmlDocument();
            document.OptionOutputAsXml = false; // Set to true if you want XML output
            document.OptionWriteEmptyNodes = true; // Set to false if you don't want to write empty nodes
            document.OptionAutoCloseOnEnd = true; // Set to false if you don't want to auto-close tags
            document.LoadHtml(content);
            using StringWriter stringWriter = new StringWriter();
            document.Save(stringWriter);
            string formattedHtml = stringWriter.ToString();
            return formattedHtml;
        }
    }

    public class JsonPostProcessor : CommonPostProcessor
    {
        public JsonPostProcessor(ILogger<JsonFileProcessor> logger) : base(logger)
        {
        }

        protected override string GetTargetExtension()
        {
            return ".json";
        }

        protected override string GetFormattedContent(string content)
        {
            System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse(content);
#pragma warning disable CA1869
            System.Text.Json.JsonSerializerOptions serializerOptions = new System.Text.Json.JsonSerializerOptions();
            serializerOptions.WriteIndented = true;
#pragma warning restore CA1869
            string formattedJson = System.Text.Json.JsonSerializer.Serialize(document, serializerOptions);
            return formattedJson;
        }
    }

    public class XmlPostProcessor : CommonPostProcessor
    {
        public XmlPostProcessor(ILogger<XmlPostProcessor> logger) : base(logger)
        {
        }

        protected override string GetTargetExtension()
        {
            return ".xml";
        }

        protected override string GetFormattedContent(string content)
        {
            System.Xml.Linq.XDocument xmlDoc = System.Xml.Linq.XDocument.Parse(content);
            // xmlDoc.Declaration = new System.Xml.Linq.XDeclaration("1.0", "utf-8", null);
            using StringWriter stringWriter = new Utf8StringWriter();
            xmlDoc.Save(stringWriter, System.Xml.Linq.SaveOptions.None);
            string formattedXml = stringWriter.ToString();
            return formattedXml;
        }

        class Utf8StringWriter : StringWriter
        {
            public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
        }
    }
}