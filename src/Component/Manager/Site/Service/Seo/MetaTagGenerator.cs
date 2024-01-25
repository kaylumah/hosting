// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    public partial class MetaTagGenerator
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Information,
            Message = "Attempting MetaTags `{Path}`")]
        private partial void LogMetaTags(string path);

        readonly ILogger _Logger;

        public MetaTagGenerator(ILogger<MetaTagGenerator> logger)
        {
            _Logger = logger;
        }

        public string ToMetaTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            LogMetaTags(renderData.Page.Uri);

            StringBuilder sb = new StringBuilder();

            string common = ToCommonTags(renderData);
            if (!string.IsNullOrEmpty(common))
            {
                sb.Append(common);
            }

            string openGraph = ToOpenGraphTags(renderData);
            if (!string.IsNullOrEmpty(openGraph))
            {
                sb.AppendLine(string.Empty);
                sb.Append(openGraph);
            }

            string twitter = ToTwitterTags(renderData);
            if (!string.IsNullOrEmpty(twitter))
            {
                sb.AppendLine(string.Empty);
                sb.Append(twitter);
            }

            string kaylumah = ToKaylumahTags(renderData);
            if (!string.IsNullOrEmpty(kaylumah))
            {
                sb.AppendLine(string.Empty);
                sb.Append(kaylumah);
            }

            return sb.ToString();
        }

        static string ToKaylumahTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            StringBuilder sb = new StringBuilder();
            SiteMetaData siteMetaData = renderData.Site;
            BuildData buildData = siteMetaData.Build;
            List<string> result = new List<string>
            {
                CreateKaylumahMetaTag("a", "b")
            };

            if (result.Count > 0)
            {
                sb.AppendLine("<!-- Kaylumah BuildInfo Meta Tags -->");
                foreach (string item in result)
                {
                    sb.AppendLine(item);
                }
            }

            return sb.ToString();
        }

        static string ToCommonTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            XmlDocument finalDocument = new XmlDocument();
            XmlElement titleElement = finalDocument.CreateElement("title");
            titleElement.InnerText = renderData.Title;

            XmlElement linkElement = finalDocument.CreateElement("link");
            XmlAttribute relAttribute = finalDocument.CreateAttribute("rel");
            relAttribute.Value = "canonical";
            linkElement.Attributes.Append(relAttribute);
            XmlAttribute hrefAttribute = finalDocument.CreateAttribute("href");
            hrefAttribute.Value = GlobalFunctions.AbsoluteUrl(renderData.Page.Uri);
            linkElement.Attributes.Append(hrefAttribute);
            StringBuilder sb = new StringBuilder();
            List<string> result = new List<string>()
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
                AuthorMetaData author = renderData.Site.AuthorMetaData[renderData.Page.Author];
                CreateMetaTag("author", author.FullName);
            }

            if (result.Count > 0)
            {
                sb.AppendLine("<!-- Common Meta Tags -->");
                foreach (string item in result)
                {
                    sb.AppendLine(item);
                }
            }

            return sb.ToString();
        }

        static string ToTwitterTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            StringBuilder sb = new StringBuilder();
            List<string> result = new List<string>
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
                OrganizationMetaData organization = renderData.Site.OrganizationMetaData[renderData.Page.Organization];
                result.Add(CreateMetaTag("twitter:site", $"@{organization.Twitter}"));
            }

            if (!string.IsNullOrEmpty(renderData.Page.Author) && renderData.Site.AuthorMetaData.Contains(renderData.Page.Author))
            {
                AuthorMetaData author = renderData.Site.AuthorMetaData[renderData.Page.Author];
                result.Add(CreateMetaTag("twitter:creator", $"@{author.Links.Twitter}"));
            }

            if (result.Count > 0)
            {
                sb.AppendLine("<!-- Twitter Meta Tags -->");
                foreach (string item in result)
                {
                    sb.AppendLine(item);
                }
            }

            return sb.ToString();
        }

        static string ToOpenGraphTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            StringBuilder sb = new StringBuilder();
            List<string> result = new List<string>
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
                    AuthorMetaData author = renderData.Site.AuthorMetaData[renderData.Page.Author];
                    result.Add(CreateOpenGraphMetaTag("article:author", author.FullName));
                }

                result.Add(CreateOpenGraphMetaTag("article:published_time", GlobalFunctions.DateToXmlschema(renderData.Page.Published)));
                result.Add(CreateOpenGraphMetaTag("article:modified_time", GlobalFunctions.DateToXmlschema(renderData.Page.Modified)));
                foreach (string tag in renderData.Page.Tags)
                {
                    result.Add(CreateOpenGraphMetaTag("article:tag", tag));
                }
            }

            if (result.Count > 0)
            {
                sb.AppendLine("<!-- OpenGraph Meta Tags -->");
                foreach (string item in result)
                {
                    sb.AppendLine(item);
                }
            }

            return sb.ToString();
        }

        static string CreateKaylumahMetaTag(string name, string content)
        {
            string namespacedName = $"kaylumah:{name}";
            return CreatePropertyMetaTag(namespacedName, content);
        }
        static string CreateOpenGraphMetaTag(string name, string content)
        {
            return CreatePropertyMetaTag(name, content);
        }

        static string CreatePropertyMetaTag(string name, string content)
        {
            return CreateMetaTag("property", name, content);
        }

        static string CreateMetaTag(string name, string content)
        {
            return CreateMetaTag("name", name, content);
        }

        static string CreateMetaTag(string idAttributeName, string name, string content)
        {
            XmlDocument finalDocument = new XmlDocument();
            XmlElement createdElement = finalDocument.CreateElement("meta");
            XmlAttribute nameAttribute = finalDocument.CreateAttribute(idAttributeName);
            nameAttribute.Value = name;
            createdElement.Attributes.Append(nameAttribute);
            XmlAttribute contentAttribute = finalDocument.CreateAttribute("content");
            contentAttribute.Value = content;
            createdElement.Attributes.Append(contentAttribute);
            return createdElement.OuterXml;
        }
    }
}
