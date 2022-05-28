// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Schema.NET;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Service.StructureData;

public class StructureDataGenerator
{
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
        _logger.LogInformation("Attempting LdJson '{Path}' and '{Type:g}'", renderData.Page.Url, renderData.Page.Type);
        if (renderData.Page.Type == ContentType.Article)
        {
            var blogPost = new BlogPosting
            {
                // Id = new Uri(GlobalFunctions.AbsoluteUrl(renderData.Page.Url)),
                MainEntityOfPage = new Values<ICreativeWork, Uri>(new Uri(GlobalFunctions.AbsoluteUrl(renderData.Page.Url))),
                Headline = renderData.Page.Title,
                DatePublished = DateTime.Parse((string)renderData.Page.PublishedDate, CultureInfo.InvariantCulture),
                DateModified = DateTime.Parse((string)renderData.Page.ModifiedDate, CultureInfo.InvariantCulture),
                Image = new Values<IImageObject, Uri>(new Uri(GlobalFunctions.AbsoluteUrl((string)renderData.Page.Image))),
                // Publisher = new Values<IOrganization, IPerson>(new Organization { })
            };

            if (authors.ContainsKey(renderData.Page.Author))
            {
                blogPost.Author = authors[renderData.Page.Author];
            }

            return blogPost.ToString(settings);
        }
        return null;
    }
}
