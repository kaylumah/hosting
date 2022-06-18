// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Text;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo;

public partial class MetaTagGenerator
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Attempting MetaTags `{Path}`")]
    private partial void LogMetaTags(string path);

    private readonly ILogger _logger;

    public MetaTagGenerator(ILogger<MetaTagGenerator> logger)
    {
        _logger = logger;
    }

    public string ToMetaTags(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        LogMetaTags(renderData.Page.Uri);

        var sb = new StringBuilder();

        sb.AppendLine("<!-- HTML Meta Tags -->");
        var openGraph = ToOpenGraphTags(renderData);
        if (!string.IsNullOrEmpty(openGraph))
        {
            sb.AppendLine(string.Empty);
            sb.Append(openGraph);
        }
        return sb.ToString();
    }

    private static string ToOpenGraphTags(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var sb = new StringBuilder();
        var result = new List<string>();
        if (renderData.Page.Type == ContentType.Article)
        {
            result.Add(CreateOpenGraphMetaTag("og:type", "article"));
        }
        if (result.Any())
        {
            sb.AppendLine("<!-- OpenGraph Meta Tags -->");
            foreach(var item in result)
            {
                sb.AppendLine(item);
            }
        }
        return sb.ToString();
    }

    private static string CreateMetaTag(string name, string content)
    {
        return CreateMetaTag("name", name, content);
    }

     private static string CreateMetaTag(string idAttributeName, string name, string content)
     {
        var finalDocument = new XmlDocument();
        var createdElement = finalDocument.CreateElement("meta");
        var nameAttribute = finalDocument.CreateAttribute(idAttributeName);
        nameAttribute.Value = name;
        createdElement.Attributes.Append(nameAttribute);
        var contentAttribute = finalDocument.CreateAttribute("content");
        contentAttribute.Value = content;
        createdElement.Attributes.Append(contentAttribute);
        return createdElement.OuterXml;
     }

    private static string CreateOpenGraphMetaTag(string name, string content)
    {
        return CreateMetaTag("property", name, content);
    }
}
