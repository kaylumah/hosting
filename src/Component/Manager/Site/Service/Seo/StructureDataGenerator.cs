// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;
using CollectionPage = Ssg.Extensions.Metadata.Abstractions.CollectionPage;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class LdJsonTargetAttribute : Attribute
    {
        public Type TargetType
        { get; }

        public LdJsonTargetAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }

    public interface ILdJsonRenderer
    {
        Thing ToLdJson(BasePage page);
    }

    [LdJsonTarget(typeof(CollectionPage))]
    public class CollectionPageLdJsonRenderer : ILdJsonRenderer
    {
        readonly Dictionary<AuthorId, Person> _Authors;
        readonly Dictionary<OrganizationId, Organization> _Organizations;

        public CollectionPageLdJsonRenderer(Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            _Authors = authors;
            _Organizations = organizations;
        }

        Thing ILdJsonRenderer.ToLdJson(BasePage page)
        {
            if (page is not CollectionPage collectionPage)
            {
                throw new InvalidOperationException();
            }

            if ("blog.html".Equals(collectionPage.Uri, StringComparison.Ordinal))
            {
                Blog blogScheme = collectionPage.ToBlog(_Authors, _Organizations);
                return blogScheme;
            }

            Schema.NET.CollectionPage collectionScheme = collectionPage.ToCollectionPage();
            return collectionScheme;
        }
    }

    [LdJsonTarget(typeof(ArticleMetaData))]
    public class ArticleMetaDataLdJsonRenderer : ILdJsonRenderer
    {
        readonly Dictionary<AuthorId, Person> _Authors;
        readonly Dictionary<OrganizationId, Organization> _Organizations;

        public ArticleMetaDataLdJsonRenderer(Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            _Authors = authors;
            _Organizations = organizations;
        }

        Thing ILdJsonRenderer.ToLdJson(BasePage page)
        {
            if (page is not ArticleMetaData article)
            {
                throw new InvalidOperationException();
            }

            BlogPosting blogPostScheme = article.ToBlogPosting(_Authors, _Organizations);
            return blogPostScheme;
        }
    }

    [LdJsonTarget(typeof(TalkMetaData))]
    public class TalkMetaDataLdJsonRenderer : ILdJsonRenderer
    {
        readonly Dictionary<AuthorId, Person> _Authors;
        readonly Dictionary<OrganizationId, Organization> _Organizations;

        public TalkMetaDataLdJsonRenderer(Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            _Authors = authors;
            _Organizations = organizations;
        }

        Thing ILdJsonRenderer.ToLdJson(BasePage page)
        {
            if (page is not TalkMetaData talk)
            {
                throw new InvalidOperationException();
            }

#pragma warning disable

            PresentationDigitalDocument presentationScheme = new PresentationDigitalDocument();
            presentationScheme.Name = "Slide Deck for Modern Microservices";
            presentationScheme.Url = new Uri("https://cdn.kaylumah.nl/slides/modern-microservices.html");
            presentationScheme.EncodingFormat = "text/html";

            Place placeScheme = new Place();
            // place.Name = "Ilionx Dev Days 2023";

            Event eventScheme = new Event();
            eventScheme.Url = talk.CanonicalUri; // new Uri("https://kaylumah.nl/talks/modern-microservices.html")
            eventScheme.Name = talk.Name; // "Modern Microservices"
            eventScheme.Description =
                talk.Description; // "Talk presented at TechConf 2025 in Amsterdam about migrating .NET monoliths to cloud-native microservices."
            string keywords = string.Join(',', talk.Tags);
            eventScheme.Keywords = keywords;
            eventScheme.WorkPerformed = presentationScheme;
            eventScheme.Location = placeScheme;
            // StartDate = new DateTimeOffset(2025, 5, 21, 14, 30, 0, TimeSpan.Zero),
            // EndDate = new DateTimeOffset(2025, 5, 21, 15, 15, 0, TimeSpan.Zero)

            if (!string.IsNullOrEmpty(talk.Author) && _Authors.TryGetValue(talk.Author, out Person? personScheme))
            {
                eventScheme.Performer = personScheme;
            }

            /*
             *
               Location = new Place
               {
                   Name = "Amsterdam RAI Conference Centre",
                   Address = new PostalAddress
                   {
                       AddressLocality = "Amsterdam",
                       AddressCountry = "NL"
                   },
                   Geo = new GeoCoordinates
                   {
                       Latitude = 52.3411,
                       Longitude = 4.8884
                   }
               }
             */

            return eventScheme;
#pragma warning restore
        }
    }

    [LdJsonTarget(typeof(PageMetaData))]
    public class PageMetaDataLdJsonRenderer : ILdJsonRenderer
    {
        readonly Dictionary<AuthorId, Person> _Authors;
        readonly Dictionary<OrganizationId, Organization> _Organizations;

        public PageMetaDataLdJsonRenderer(Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            _Authors = authors;
            _Organizations = organizations;
        }

        Thing ILdJsonRenderer.ToLdJson(BasePage page)
        {
            if (page is not PageMetaData pageMetaData)
            {
                throw new InvalidOperationException();
            }

            WebSite scheme = new WebSite();
            scheme.Name = pageMetaData.Title;
            // scheme.Url = new Uri(renderData.Site.Url);
            WebPage webPageScheme = new WebPage();
            webPageScheme.Name = pageMetaData.Title;
            webPageScheme.Url = page.CanonicalUri;
            webPageScheme.Description = pageMetaData.Description;
            webPageScheme.IsPartOf = scheme;
            return webPageScheme;
        }
    }

    public partial class StructureDataGenerator
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Trace,
            Message = "Attempting LdJson `{Path}` and `{Type}`")]
        private partial void LogLdJson(string path, string type);

        readonly IServiceProvider _ServiceProvider;
        readonly ILogger _Logger;

        static readonly Dictionary<Type, Type> _Map;

        static StructureDataGenerator()
        {
            Type type = typeof(ILdJsonRenderer);
            Assembly assembly = type.Assembly;
            Type[] types = assembly.GetImplementationsForType(type);

            _Map = new Dictionary<Type, Type>();
            foreach (Type ldJsonRendererType in types)
            {
                LdJsonTargetAttribute? attribute = ldJsonRendererType.GetCustomAttribute<LdJsonTargetAttribute>();
                if (attribute != null)
                {
                    Type pageType = attribute.TargetType;
                    _Map.Add(pageType, ldJsonRendererType);
                }
            }
        }

        public StructureDataGenerator(IServiceProvider serviceProvider, ILogger<StructureDataGenerator> logger)
        {
            _ServiceProvider = serviceProvider;
            _Logger = logger;
        }

        public string ToLdJson(RenderData renderData)
        {
            // Check https://search.google.com/test/rich-results to validate LDJson
            ArgumentNullException.ThrowIfNull(renderData);
            System.Text.Json.JsonSerializerOptions settings = new System.Text.Json.JsonSerializerOptions();
            settings.AllowTrailingCommas = true;
            settings.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
            settings.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            settings.WriteIndented = true;

            Dictionary<AuthorId, Person> authors = renderData.Site.ToPersons();
            Dictionary<OrganizationId, Organization> organizations = renderData.Site.ToOrganizations();

            Type pageType = renderData.Page.GetType();
            bool hasConverter = _Map.TryGetValue(pageType, out Type? converterType);
            if (hasConverter && converterType != null)
            {
                bool implementsILdJsonRenderer = typeof(ILdJsonRenderer).IsAssignableFrom(converterType);
                Debug.Assert(implementsILdJsonRenderer);
                object[] arguments = [authors, organizations];
                object renderer = ActivatorUtilities.CreateInstance(_ServiceProvider, converterType, arguments);
                if (renderer is ILdJsonRenderer ldJsonRenderer)
                {
                    Thing scheme = ldJsonRenderer.ToLdJson(renderData.Page);
                    string json = scheme.ToString(settings);
                    return json;
                }
            }

            return string.Empty;
        }
    }
}
