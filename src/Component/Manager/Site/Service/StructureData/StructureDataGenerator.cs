﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Schema.NET;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Service.StructureData;

public static class StructureDataGenerator
{
    public static string ToLdJson(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        var settings = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        if (renderData.Page.Type == ContentType.Article)
        {
            var blogPost = new BlogPosting
            {
                // Id = new Uri(GlobalFunctions.AbsoluteUrl(renderData.Page.Url)),
                MainEntityOfPage = new Values<ICreativeWork, Uri>(new Uri(GlobalFunctions.AbsoluteUrl(renderData.Page.Url))),
                Headline = renderData.Page.Title,
                DatePublished = DateTime.Parse((string)renderData.Page["publisheddate"], CultureInfo.InvariantCulture),
                DateModified = DateTime.Parse((string)renderData.Page["modifieddate"], CultureInfo.InvariantCulture),
                Image = new Values<IImageObject, Uri>(new Uri(GlobalFunctions.AbsoluteUrl((string)renderData.Page["image"]))),
                Author = new Values<IOrganization, IPerson>(new Person
                {
                    Name = new OneOrMany<string>("Max Hamulyák"),
                    Url = new OneOrMany<Uri>(new Uri(GlobalFunctions.Instance.Url))
                }),
                Publisher = new Values<IOrganization, IPerson>(new Organization { })
            };
            return blogPost.ToString(settings);
        }
        return null;
    }
}
