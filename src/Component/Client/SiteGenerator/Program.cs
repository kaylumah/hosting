using System;
using System.IO;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Access.Artifact.Service;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
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
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Environment.CurrentDirectory, "_site"))
            );
            services.AddSingleton<IArtifactAccess, ArtifactAccess>();
            services.AddSingleton<ISiteManager, SiteManager>();
            var serviceProvider = services.BuildServiceProvider();
            var siteManager = serviceProvider.GetRequiredService<ISiteManager>();
            await siteManager.GenerateSite();
        }
    }
}
