// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.Extensions.Metadata.YamlFrontMatter;
using Kaylumah.Ssg.iFX.Data.Csv;
using Kaylumah.Ssg.iFX.Data.Json;
using Kaylumah.Ssg.iFX.Data.Yaml;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.Seo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Hosting
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSiteManager(this IServiceCollection services, IConfiguration configuration)
        {
            ServiceDescriptor loggerDescriptor = ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>));
            services.TryAdd(loggerDescriptor);

            services.SetupOptions<SiteInfo>(configuration, "Site");
            services.SetupOptions<MetadataParserOptions>(configuration, MetadataParserOptions.Options);

            services.AddSingleton<IFileProcessor, FileProcessor>();
            services.AddSingleton<IFrontMatterMetadataProvider, YamlFrontMatterMetadataProvider>();
            services.AddSingleton<IYamlParser, YamlParser>();
            services.AddSingleton<IJsonParser, JsonParser>();
            services.AddSingleton<ICsvParser, CsvParser>();
            services.AddSingleton<StructureDataGenerator>();
            services.AddSingleton<MetaTagGenerator>();
            services.TryAddSingleton(TimeProvider.System);
            services.RegisterImplementationsAsSingleton<IContentPreprocessorStrategy>();
            services.RegisterImplementationsAsSingleton<IRenderPlugin>();
            services.RegisterImplementationsAsSingleton<IKnownFileProcessor>();
            services.RegisterImplementationsAsSingleton<IKnownExtensionProcessor>();
            services.RegisterImplementationsAsSingleton<IPostProcessor>();

            services.AddProxiedService<ISiteManager, SiteManager>();

            return services;
        }
    }
}
