// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.Seo;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;

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

            services.AddSingleton<IContentPreprocessorStrategy, MarkdownContentPreprocessorStrategy>();
            services.AddSingleton<IFileProcessor, FileProcessor>();
            services.AddSingleton<IFrontMatterMetadataProvider, YamlFrontMatterMetadataProvider>();
            services.AddSingleton<IYamlParser, YamlParser>();
            services.AddSingleton<ISiteManager, SiteManager>();
            services.AddSingleton<DataProcessor>();
            services.AddSingleton<FeedGenerator>();
            services.AddSingleton<StructureDataGenerator>();
            services.AddSingleton<MetaTagGenerator>();
            services.AddSingleton<SeoGenerator>();
            services.AddSingleton<SiteMapGenerator>();
            services.AddSingleton<IRenderPlugin, HtmlSeoRenderPlugin>();
            services.AddSingleton<ISiteArtifactPlugin, SiteMapSiteArtifactPlugin>();
            services.AddSingleton<ISiteArtifactPlugin, FeedSiteArtifactPlugin>();
            services.AddSingleton<ISiteArtifactPlugin, SearchIndexArtifactPlugin>();
            services.AddSingleton<IKnownFileProcessor, TagFileProcessor>();
            services.AddSingleton<IKnownFileProcessor, OrganizationFileProcessor>();
            services.AddSingleton<IKnownFileProcessor, AuthorFileProcessor>();
            services.AddSingleton<IKnownExtensionProcessor, YamlFileProcessor>();
            services.AddSingleton(TimeProvider.System);
            return services;
        }
    }
}
