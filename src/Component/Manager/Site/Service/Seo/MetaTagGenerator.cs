// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Microsoft.Extensions.Logging;
using System.Xml;

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
        return string.Empty;
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
