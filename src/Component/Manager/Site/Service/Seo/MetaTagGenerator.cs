// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Xml;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;

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
        var finalDocument = new XmlDocument();
        var titleElement = finalDocument.CreateElement("title");
        titleElement.InnerText = renderData.Title;

        var linkElement = finalDocument.CreateElement("link");
        var relAttribute = finalDocument.CreateAttribute("rel");
        relAttribute.Value = "canonical";
        linkElement.Attributes.Append(relAttribute);
        var hrefAttribute = finalDocument.CreateAttribute("href");
        hrefAttribute.Value = GlobalFunctions.AbsoluteUrl(renderData.Page.Uri);
        linkElement.Attributes.Append(hrefAttribute);
        var sb = new StringBuilder();
        var result = new List<string>()
        {
            titleElement.OuterXml,
            linkElement.OuterXml,
            CreateMetaTag("generator", $"Kaylumah v{renderData.Site.Build.ShortGitHash}"),
            CreateMetaTag("description", renderData.Description),
            CreateMetaTag("copyright", renderData.Site.Build.Copyright),
            CreateMetaTag("keywords", string.Join(", ", renderData.Page.Tags))
        };
        if (!string.IsNullOrEmpty(renderData.Page.Author) && renderData.Site.AuthorMetaData.Contains(renderData.Page.Author))
        {
            var author = renderData.Site.AuthorMetaData[renderData.Page.Author];
            CreateMetaTag("author", author.FullName);
        }

        if (result.Any())
        {
            sb.AppendLine("<!-- Common Meta Tags -->");
            foreach (var item in result)
            {
                sb.AppendLine(item);
            }
        }
        return sb.ToString();
    }

    private static string ToTwitterTags(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var sb = new StringBuilder();
        var result = new List<string>
        {
            CreateMetaTag("twitter:card", "summary_large_image"),
            CreateMetaTag("twitter:title", renderData.Page.Title),
            CreateMetaTag("twitter:description", renderData.Description)
        };

        if (!string.IsNullOrEmpty(renderData.Page.Image))
        {
            result.Add(CreateMetaTag("twitter:image", GlobalFunctions.AbsoluteUrl(renderData.Page.Image)));
        }

        if (!string.IsNullOrEmpty(renderData.Page.Organization))
        {
            var organization = renderData.Site.OrganizationMetaData[renderData.Page.Organization];
            result.Add(CreateMetaTag("twitter:site", $"@{organization.Twitter}"));
        }
        if (!string.IsNullOrEmpty(renderData.Page.Author) && renderData.Site.AuthorMetaData.Contains(renderData.Page.Author))
        {
            var author = renderData.Site.AuthorMetaData[renderData.Page.Author];
            result.Add(CreateMetaTag("twitter:creator", $"@{author.Links.Twitter}"));
        }

        if (result.Any())
        {
            sb.AppendLine("<!-- Twitter Meta Tags -->");
            foreach (var item in result)
            {
                sb.AppendLine(item);
            }
        }
        return sb.ToString();
    }

    private static string ToOpenGraphTags(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var sb = new StringBuilder();
        var result = new List<string>
        {
            CreateOpenGraphMetaTag("og:type", renderData.Page.Type == ContentType.Article ? "article" : "website"),
            CreateOpenGraphMetaTag("og:locale", renderData.Language),
            CreateOpenGraphMetaTag("og:site_name", renderData.Site.Title),
            CreateOpenGraphMetaTag("og:title", renderData.Page.Title),
            CreateOpenGraphMetaTag("og:url", GlobalFunctions.AbsoluteUrl(renderData.Page.Uri)),
            CreateOpenGraphMetaTag("og:description", renderData.Description)
        };

        if (!string.IsNullOrEmpty(renderData.Page.Image))
        {
            result.Add(CreateOpenGraphMetaTag("og:image", GlobalFunctions.AbsoluteUrl(renderData.Page.Image)));
        }

        if (renderData.Page.Type == ContentType.Article)
        {
            if (!string.IsNullOrEmpty(renderData.Page.Author) && renderData.Site.AuthorMetaData.Contains(renderData.Page.Author))
            {
                var author = renderData.Site.AuthorMetaData[renderData.Page.Author];
                result.Add(CreateOpenGraphMetaTag("article:author", author.FullName));
            }

            result.Add(CreateOpenGraphMetaTag("article:published_time", GlobalFunctions.DateToXmlschema(renderData.Page.Published)));
            result.Add(CreateOpenGraphMetaTag("article:modified_time", GlobalFunctions.DateToXmlschema(renderData.Page.Modified)));
            foreach (var tag in renderData.Page.Tags)
            {
                result.Add(CreateOpenGraphMetaTag("article:tag", tag));
            }
        }

        if (result.Any())
        {
            sb.AppendLine("<!-- OpenGraph Meta Tags -->");
            foreach (var item in result)
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
