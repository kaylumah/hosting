// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using Kaylumah.Ssg.Engine.Transformation.Interface;
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
            var blogPost = renderData.Page.ToBlogPosting(authors, organizations);
            return blogPost.ToString(settings);
        }
        else if (renderData.Page.Type == ContentType.Page && "blog.html".Equals(renderData.Page.Uri, StringComparison.Ordinal))
        {
            var posts = renderData.Site.Pages.IsArticle().ToBlogPostings(authors, organizations).ToList();
            var blog = new Blog()
            {
                BlogPost = new OneOrMany<IBlogPosting>(posts)
            };
            return blog.ToString(settings);
        }
        return null;
    }
}
