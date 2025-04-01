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

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
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
