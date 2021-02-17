using System;
using System.IO;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Access.Artifact.Service;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Client.SiteGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddFileSystem(Path.Combine(Environment.CurrentDirectory, "_site"));
            services.AddSingleton<IArtifactAccess, ArtifactAccess>();
            services.AddSingleton<ISiteManager, SiteManager>();
            var serviceProvider = services.BuildServiceProvider();
            var siteManager = serviceProvider.GetRequiredService<ISiteManager>();
            await siteManager.GenerateSite();
        }
    }

    static class FileSystemServiceCollectionExtensions
    {
        public static IServiceCollection AddFileSystem(this IServiceCollection services, string rootDirectory)
        {
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(rootDirectory));
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IFileProcessor, CustomFileProcessor>();
            return services;
        }
    }
}
