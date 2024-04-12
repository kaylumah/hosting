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
            StringBuilder sb = new StringBuilder();
            if (renderData.Page is PageMetaData pageMetaData)
            {
                LogMetaTags(pageMetaData.Uri);

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
            }

            string result = sb.ToString();
            return result;
        }

        static string ToKaylumahTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            StringBuilder sb = new StringBuilder();
            SiteMetaData siteMetaData = renderData.Site;
            if (renderData.Page is PageMetaData pageMetaData)
            {
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
            }

            string sbResult = sb.ToString();
            return sbResult;
        }

        static Uri AbsoluteUri(string url)
        {
            Uri absolute = GlobalFunctions.AbsoluteUri(url);
            return absolute;
        }

        static string ToCommonTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            StringBuilder sb = new StringBuilder();

            if (renderData.Page is PageMetaData pageMetaData)
            {
                XmlDocument finalDocument = new XmlDocument();
                XmlElement titleElement = finalDocument.CreateElement("title");
                titleElement.InnerText = renderData.Title;
                Uri pageUri = pageMetaData.Canonical;
                Uri feedUri = AbsoluteUri("feed.xml");
                Uri sitemapUri = AbsoluteUri("sitemap.xml");
                string formattedTags = string.Join(", ", pageMetaData.Tags);
                List<string> result = new List<string>()
                {
                    titleElement.OuterXml,
                    CreateLinkTag("canonical", pageUri),
                    CreateLinkTag("alternate", feedUri, "application/atom+xml", $"{renderData.Site.Title} RSS Feed"),
                    CreateLinkTag("sitemap", sitemapUri, "application/xml", $"{renderData.Site.Title} Sitemap"),
                    CreateMetaTag("generator", $"Kaylumah v{renderData.Site.Build.ShortGitHash}"),
                    CreateMetaTag("description", renderData.Description),
                    CreateMetaTag("copyright", renderData.Site.Build.Copyright),
                    CreateMetaTag("keywords", formattedTags)
                };
                if (!string.IsNullOrEmpty(pageMetaData.Author) && renderData.Site.AuthorMetaData.Contains(pageMetaData.Author))
                {
                    AuthorMetaData author = renderData.Site.AuthorMetaData[pageMetaData.Author];
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
            }

            string sbResult = sb.ToString();
            return sbResult;
        }

        static string ToTwitterTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);

            StringBuilder sb = new StringBuilder();
            if (renderData.Page is PageMetaData pageMetaData)
            {
                List<string> result = new List<string>
        {
            CreateMetaTag("twitter:card", "summary_large_image"),
            CreateMetaTag("twitter:title", pageMetaData.Title),
            CreateMetaTag("twitter:description", renderData.Description)
        };

                if (pageMetaData.WebImage != null)
                {
                    Uri twitterUri = pageMetaData.WebImage;
                    string url = twitterUri.ToString();
                    string imageTag = CreateMetaTag("twitter:image", url);
                    result.Add(imageTag);
                }

                if (!string.IsNullOrEmpty(pageMetaData.Organization))
                {
                    OrganizationMetaData organization = renderData.Site.OrganizationMetaData[pageMetaData.Organization];
                    string organizationTag = CreateMetaTag("twitter:site", $"@{organization.Twitter}");
                    result.Add(organizationTag);
                }

                if (!string.IsNullOrEmpty(pageMetaData.Author) && renderData.Site.AuthorMetaData.Contains(pageMetaData.Author))
                {
                    AuthorMetaData author = renderData.Site.AuthorMetaData[pageMetaData.Author];
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
            }

            string sbResult = sb.ToString();
            return sbResult;
        }

        static string ToOpenGraphTags(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            StringBuilder sb = new StringBuilder();
            if (renderData.Page is PageMetaData pageMetaData)
            {
                Uri pageUri = pageMetaData.Canonical;
                string pageUrl = pageUri.ToString();
                List<string> result = new List<string>
        {
            CreateOpenGraphMetaTag("og:type", pageMetaData.IsArticle() ? "article" : "website"),
            CreateOpenGraphMetaTag("og:locale", renderData.Language),
            CreateOpenGraphMetaTag("og:site_name", renderData.Site.Title),
            CreateOpenGraphMetaTag("og:title", pageMetaData.Title),
            CreateOpenGraphMetaTag("og:url", pageUrl),
            CreateOpenGraphMetaTag("og:description", renderData.Description)
        };

                if (pageMetaData.WebImage != null)
                {
                    Uri image = pageMetaData.WebImage;
                    string imageUrl = image.ToString();
                    string imageTag = CreateOpenGraphMetaTag("og:image", imageUrl);
                    result.Add(imageTag);
                }

                if (pageMetaData.IsArticle())
                {
                    if (!string.IsNullOrEmpty(pageMetaData.Author) && renderData.Site.AuthorMetaData.Contains(pageMetaData.Author))
                    {
                        AuthorMetaData author = renderData.Site.AuthorMetaData[pageMetaData.Author];
                        string authorTag = CreateOpenGraphMetaTag("article:author", author.FullName);
                        result.Add(authorTag);
                    }

                    string formattedPublishTime = GlobalFunctions.DateToXmlschema(pageMetaData.Published);
                    string formattedModifiedTime = GlobalFunctions.DateToXmlschema(pageMetaData.Modified);
                    string publishedTag = CreateOpenGraphMetaTag("article:published_time", formattedPublishTime);
                    string modifiedTag = CreateOpenGraphMetaTag("article:modified_time", formattedModifiedTime);
                    result.Add(publishedTag);
                    result.Add(modifiedTag);
                    foreach (string tag in pageMetaData.Tags)
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

        static string CreateLinkTag(string rel, Uri url, string? type = null, string? title = null)
        {
            XmlDocument finalDocument = new XmlDocument();
            XmlElement linkElement = finalDocument.CreateElement("link");

            XmlAttribute relAttribute = finalDocument.CreateAttribute("rel");
            relAttribute.Value = rel;
            linkElement.Attributes.Append(relAttribute);

            if (string.IsNullOrEmpty(type) == false)
            {
                XmlAttribute typeAttribute = finalDocument.CreateAttribute("type");
                typeAttribute.Value = type;
                linkElement.Attributes.Append(typeAttribute);
            }

            XmlAttribute urlAttribute = finalDocument.CreateAttribute("href");
            urlAttribute.Value = url.ToString();
            linkElement.Attributes.Append(urlAttribute);

            if (string.IsNullOrEmpty(title) == false)
            {
                XmlAttribute titleAttribute = finalDocument.CreateAttribute("title");
                titleAttribute.Value = title;
                linkElement.Attributes.Append(titleAttribute);
            }

            string result = linkElement.OuterXml;
            return result;
        }
    }
}
