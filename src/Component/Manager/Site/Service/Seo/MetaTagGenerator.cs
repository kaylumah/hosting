// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Text;
using Kaylumah.Ssg.Utilities;

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

        var common = ToCommonTags(renderData);
        if (!string.IsNullOrEmpty(common))
        {
            sb.Append(common);
        }

        var openGraph = ToOpenGraphTags(renderData);
        if (!string.IsNullOrEmpty(openGraph))
        {
            sb.AppendLine(string.Empty);
            sb.Append(openGraph);
        }
        var twitter = ToTwitterTags(renderData);
        if (!string.IsNullOrEmpty(twitter))
        {
            sb.AppendLine(string.Empty);
            sb.Append(twitter);
        }
        return sb.ToString();
    }

    private static string ToCommonTags(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var author = renderData.Site.AuthorMetaData[renderData.Page.Author];
        var finalDocument = new XmlDocument();
        var titleElement = finalDocument.CreateElement("title");
        titleElement.InnerText = renderData.Title;

        var linkElement = finalDocument.CreateElement("link");
        var hrefAttribute = finalDocument.CreateAttribute("href");
        hrefAttribute.Value = renderData.Page.Uri;
        createdElement.Attributes.Append(hrefAttribute);
        var relAttribute = finalDocument.CreateAttribute("rel");
        relAttribute.Value = "canonical";
        createdElement.Attributes.Append(nameAttribute);
        var sb = new StringBuilder();
        var result = new List<string>()
        {
            titleElement.OuterXml,
            linkElement.OuterXml,
            CreateMetaTag("generator", $"Kaylumah v{renderData.Site.Build.ShortGitHash}"),
            CreateMetaTag("description", renderData.Description),
            CreateMetaTag("author", author.FullName),
            CreateMetaTag("copyright", renderData.Site.Build.Copyright),
            // <!-- <meta name="keywords" content="{{ page.tags | array.join "," }}"> -->
        };
        if (result.Any())
        {
            sb.AppendLine("<!-- Common Meta Tags -->");
            foreach(var item in result)
            {
                sb.AppendLine(item);
            }
        }
        return sb.ToString();
    }

    private static string ToTwitterTags(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var author = renderData.Site.AuthorMetaData[renderData.Page.Author];
        var organization = renderData.Site.OrganizationMetaData[renderData.Page.Organization];
        var sb = new StringBuilder();
        var result = new List<string>
        {
            CreateMetaTag("twitter:card", "summary_large_image"),
            CreateMetaTag("twitter:title", renderData.Page.Title),
            CreateMetaTag("twitter:site", $"@{organization.Twitter}"),
            CreateMetaTag("twitter:creator", $"@{author.Links.Twitter}"),
            CreateMetaTag("twitter:description", renderData.Description),
            CreateMetaTag("twitter:image", renderData.Page.Image)
        };

        if (result.Any())
        {
            sb.AppendLine("<!-- Twitter Meta Tags -->");
            foreach(var item in result)
            {
                sb.AppendLine(item);
            }
        }
        return sb.ToString();
    }

    private static string ToOpenGraphTags(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var author = renderData.Site.AuthorMetaData[renderData.Page.Author];
        var sb = new StringBuilder();
        var result = new List<string>
        {
            CreateOpenGraphMetaTag("og:type", renderData.Page.Type == ContentType.Article ? "article" : "website"),
            CreateOpenGraphMetaTag("og:locale", renderData.Language),
            CreateOpenGraphMetaTag("og:site_name", renderData.Site.Title),
            CreateOpenGraphMetaTag("og:title", renderData.Page.Title),
            CreateOpenGraphMetaTag("og:url", renderData.Page.Uri),
            CreateOpenGraphMetaTag("og:image", renderData.Page.Image),
            CreateOpenGraphMetaTag("og:description", renderData.Description)
        };

        if (renderData.Page.Type == ContentType.Article)
        {
            result.Add(CreateOpenGraphMetaTag("article:author", author.FullName));
            result.Add(CreateOpenGraphMetaTag("article:published_time", GlobalFunctions.DateToXmlschema(renderData.Page.Published)));
            result.Add(CreateOpenGraphMetaTag("article:modified_time", GlobalFunctions.DateToXmlschema(renderData.Page.Modified)));
            foreach(var tag in renderData.Page.Tags)
            {
                result.Add(CreateOpenGraphMetaTag("article:tag", tag));
            }
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
