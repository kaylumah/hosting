using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Access.Artifact.Service;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Client.SiteGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "settings.json"))
                .AddInMemoryCollection(new Dictionary<string, string> {
                    { $"{nameof(SiteConfiguration)}:Source", "_site" },
                    { $"{nameof(SiteConfiguration)}:Destination", "dist" },
                    { $"{nameof(SiteConfiguration)}:LayoutDirectory", "_layouts" },
                    { $"{nameof(SiteConfiguration)}:PartialsDirectory", "_includes" },
                    { $"{nameof(SiteConfiguration)}:DataDirectory", "_data" },
                    { $"{nameof(SiteConfiguration)}:AssetDirectory", "assets" }
                });
            IConfiguration configuration = configurationBuilder.Build();

            IServiceCollection services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddFileSystem(configuration, Path.Combine(Environment.CurrentDirectory,  "_site"));
            services.AddSingleton<IArtifactAccess, ArtifactAccess>();
            services.AddSingleton<ISiteManager, SiteManager>();
            var serviceProvider = services.BuildServiceProvider();
            var siteManager = serviceProvider.GetRequiredService<ISiteManager>();

            var siteConfiguration = new SiteConfiguration();
            configuration.GetSection(nameof(SiteConfiguration)).Bind(siteConfiguration);
            
            await siteManager.GenerateSite(new GenerateSiteRequest {
                Configuration = siteConfiguration
            });
        }
    }

    static class FileSystemServiceCollectionExtensions
    {
        public static IServiceCollection AddFileSystem(this IServiceCollection services, IConfiguration configuration, string rootDirectory)
        {
            services.Configure<SiteInfo>(configuration.GetSection("X"));
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(rootDirectory));
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IContentPreprocessorStrategy, MarkdownContentPreprocessorStrategy>();
            services.AddSingleton<IFileProcessor, FileProcessor>();
            return services;
        }
    }
}
