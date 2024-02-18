// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
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
            Level = LogLevel.Trace,
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

            string result = sb.ToString();
            return result;
        }

        static string ToKaylumahTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            StringBuilder sb = new StringBuilder();
            SiteMetaData siteMetaData = renderData.Site;
            PageMetaData pageMetaData = renderData.Page;
            BuildData buildData = siteMetaData.Build;
            string formattedTime = buildData.Time.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
            List<string> result = new List<string>
            {
                CreateKaylumahMetaTag("copyright", buildData.Copyright),
                CreateKaylumahMetaTag("commit", buildData.GitHash),
                CreateKaylumahMetaTag("version", buildData.Version),
                CreateKaylumahMetaTag("buildId", buildData.BuildId),
                CreateKaylumahMetaTag("buildNumber", buildData.BuildNumber),
                CreateKaylumahMetaTag("time", formattedTime),
                CreateKaylumahMetaTag("site", siteMetaData.Id),
                CreateKaylumahMetaTag("page", pageMetaData.Id)
            };

            if (0 < result.Count)
            {
                sb.AppendLine("<!-- Kaylumah BuildInfo Meta Tags -->");
                foreach (string item in result)
                {
                    sb.AppendLine(item);
                }
            }

            string sbResult = sb.ToString();
            return sbResult;
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
            string formattedTags = string.Join(", ", renderData.Page.Tags);
            StringBuilder sb = new StringBuilder();
            List<string> result = new List<string>()
        {
            titleElement.OuterXml,
            linkElement.OuterXml,
            CreateMetaTag("generator", $"Kaylumah v{renderData.Site.Build.ShortGitHash}"),
            CreateMetaTag("description", renderData.Description),
            CreateMetaTag("copyright", renderData.Site.Build.Copyright),
            CreateMetaTag("keywords", formattedTags)
        };
            if (!string.IsNullOrEmpty(renderData.Page.Author) && renderData.Site.AuthorMetaData.Contains(renderData.Page.Author))
            {
                AuthorMetaData author = renderData.Site.AuthorMetaData[renderData.Page.Author];
                CreateMetaTag("author", author.FullName);
            }

            if (0 < result.Count)
            {
                sb.AppendLine("<!-- Common Meta Tags -->");
                foreach (string item in result)
                {
                    sb.AppendLine(item);
                }
            }

            string sbResult = sb.ToString();
            return sbResult;
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
                string twitterUrl = GlobalFunctions.AbsoluteUrl(renderData.Page.Image);
                string imageTag = CreateMetaTag("twitter:image", twitterUrl);
                result.Add(imageTag);
            }

            if (!string.IsNullOrEmpty(renderData.Page.Organization))
            {
                OrganizationMetaData organization = renderData.Site.OrganizationMetaData[renderData.Page.Organization];
                string organizationTag = CreateMetaTag("twitter:site", $"@{organization.Twitter}");
                result.Add(organizationTag);
            }

            if (!string.IsNullOrEmpty(renderData.Page.Author) && renderData.Site.AuthorMetaData.Contains(renderData.Page.Author))
            {
                AuthorMetaData author = renderData.Site.AuthorMetaData[renderData.Page.Author];
                string creatorTag = CreateMetaTag("twitter:creator", $"@{author.Links.Twitter}");
                result.Add(creatorTag);
            }

            if (0 < result.Count)
            {
                sb.AppendLine("<!-- Twitter Meta Tags -->");
                foreach (string item in result)
                {
                    sb.AppendLine(item);
                }
            }

            string sbResult = sb.ToString();
            return sbResult;
        }

        static string ToOpenGraphTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            StringBuilder sb = new StringBuilder();
            string pageUrl = GlobalFunctions.AbsoluteUrl(renderData.Page.Uri);
            List<string> result = new List<string>
        {
            CreateOpenGraphMetaTag("og:type", renderData.Page.IsArticle() ? "article" : "website"),
            CreateOpenGraphMetaTag("og:locale", renderData.Language),
            CreateOpenGraphMetaTag("og:site_name", renderData.Site.Title),
            CreateOpenGraphMetaTag("og:title", renderData.Page.Title),
            CreateOpenGraphMetaTag("og:url", pageUrl),
            CreateOpenGraphMetaTag("og:description", renderData.Description)
        };

            if (!string.IsNullOrEmpty(renderData.Page.Image))
            {
                string imageUrl = GlobalFunctions.AbsoluteUrl(renderData.Page.Image);
                string imageTag = CreateOpenGraphMetaTag("og:image", imageUrl);
                result.Add(imageTag);
            }

            if (renderData.Page.IsArticle())
            {
                if (!string.IsNullOrEmpty(renderData.Page.Author) && renderData.Site.AuthorMetaData.Contains(renderData.Page.Author))
                {
                    AuthorMetaData author = renderData.Site.AuthorMetaData[renderData.Page.Author];
                    string authorTag = CreateOpenGraphMetaTag("article:author", author.FullName);
                    result.Add(authorTag);
                }

                string formattedPublishTime = GlobalFunctions.DateToXmlschema(renderData.Page.Published);
                string formattedModifiedTime = GlobalFunctions.DateToXmlschema(renderData.Page.Modified);
                string publishedTag = CreateOpenGraphMetaTag("article:published_time", formattedPublishTime);
                string modifiedTag = CreateOpenGraphMetaTag("article:modified_time", formattedModifiedTime);
                result.Add(publishedTag);
                result.Add(modifiedTag);
                foreach (string tag in renderData.Page.Tags)
                {
                    string htmlTag = CreateOpenGraphMetaTag("article:tag", tag);
                    result.Add(htmlTag);
                }
            }

            if (0 < result.Count)
            {
                sb.AppendLine("<!-- OpenGraph Meta Tags -->");
                foreach (string item in result)
                {
                    sb.AppendLine(item);
                }
            }

            string sbResult = sb.ToString();
            return sbResult;
        }

        static string CreateKaylumahMetaTag(string name, string content)
        {
            string namespacedName = $"kaylumah:{name}";
            string result = CreatePropertyMetaTag(namespacedName, content);
            return result;
        }
        static string CreateOpenGraphMetaTag(string name, string content)
        {
            string result = CreatePropertyMetaTag(name, content);
            return result;
        }

        static string CreatePropertyMetaTag(string name, string content)
        {
            string result = CreateMetaTag("property", name, content);
            return result;
        }

        static string CreateMetaTag(string name, string content)
        {
            string result = CreateMetaTag("name", name, content);
            return result;
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
            string result = createdElement.OuterXml;
            return result;
        }
    }
}
