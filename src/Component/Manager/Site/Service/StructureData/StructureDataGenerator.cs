// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Schema.NET;

namespace Kaylumah.Ssg.Manager.Site.Service.StructureData;

public partial class StructureDataGenerator
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Attempting LdJson `{Path}` and `{Type:g}`")]
    private partial void LogLdJson(string path, ContentType type);

    private readonly ILogger _logger;

    public StructureDataGenerator(ILogger<StructureDataGenerator> logger)
    {
        _logger = logger;
    }

    public string ToLdJson(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var settings = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        var authors = renderData.Site.ToPersons();
        var organizations = renderData.Site.ToOrganizations();
        LogLdJson(renderData.Page.Uri, renderData.Page.Type);
        if (renderData.Page.Type == ContentType.Article)
        {
            var blogPost = new BlogPosting
            {
                // Id = new Uri(GlobalFunctions.AbsoluteUrl(renderData.page.Uri)),
                MainEntityOfPage = new Values<ICreativeWork, Uri>(new Uri(GlobalFunctions.AbsoluteUrl(renderData.Page.Uri))),
                Headline = renderData.Page.Title,
#pragma warning disable RS0030 // datetime is expected here
                DatePublished = renderData.Page.Published.DateTime,
                DateModified = renderData.Page.Modified.DateTime,
#pragma warning restore RS0030 // datetime is expected here
                Image = new Values<IImageObject, Uri>(new Uri(GlobalFunctions.AbsoluteUrl((string)renderData.Page.Image))),
                // Publisher = new Values<IOrganization, IPerson>(new Organization { })
            };

            if (authors.ContainsKey(renderData.Page.Author))
            {
                blogPost.Author = authors[renderData.Page.Author];
            }

            if (organizations.ContainsKey(renderData.Page.Organization))
            {
                blogPost.Publisher = organizations[renderData.Page.Organization];
            }

            return blogPost.ToString(settings);
        }
        else if (renderData.Page.Type == ContentType.Page && "blog.html".Equals(renderData.Page.Uri, StringComparison.Ordinal))
        {
            var blog = new Blog()
            {
            };
            return blog.ToString(settings);
        }
        return null;
    }
}
