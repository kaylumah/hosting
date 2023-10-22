
// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.Seo;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Kaylumah.Ssg.Utilities.Time;
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
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

            services.SetupOptions<SiteInfo>(configuration, "Site");
            services.SetupOptions<MetadataParserOptions>(configuration, MetadataParserOptions.Options);

            services.AddSingleton<IContentPreprocessorStrategy, MarkdownContentPreprocessorStrategy>();
            services.AddSingleton<IFileMetadataParser, FileMetadataParser>();
            services.AddSingleton<IFileProcessor, FileProcessor>();
            services.AddSingleton<IMetadataProvider, YamlFrontMatterMetadataProvider>();
            services.AddSingleton<IYamlParser, YamlParser>();
            services.AddSingleton<ISiteManager, SiteManager>();
            services.AddSingleton<SiteMetadataFactory>();
            services.AddSingleton<FeedGenerator>();
            services.AddSingleton<StructureDataGenerator>();
            services.AddSingleton<MetaTagGenerator>();
            services.AddSingleton<SeoGenerator>();
            services.AddSingleton<SiteMapGenerator>();
            services.AddSystemClock();
            services.AddSingleton<IRenderPlugin, HtmlSeoRenderPlugin>();
            services.AddSingleton<ISiteArtifactPlugin, SiteMapSiteArtifactPlugin>();
            services.AddSingleton<ISiteArtifactPlugin, FeedSiteArtifactPlugin>();
            return services;
        }
    }
}
