﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Hosting;
using Kaylumah.Ssg.Manager.Site.Hosting;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Utilities.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Client.SiteGenerator
{
    sealed class Program
    {
        static void ShowKaylumahLogo()
        {
            string applicationName = typeof(Program).Namespace!;
            const string message = @"
  _  __           _                       _     
 | |/ /__ _ _   _| |_   _ _ __ ___   __ _| |__  
 | ' // _` | | | | | | | | '_ ` _ \ / _` | '_ \ 
 | . \ (_| | |_| | | |_| | | | | | | (_| | | | |
 |_|\_\__,_|\__, |_|\__,_|_| |_| |_|\__,_|_| |_|
            |___/                               ";

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(message);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(applicationName);
            Console.ResetColor();
            Console.WriteLine();
        }

        static async Task Main(string[] args)
        {
            ShowKaylumahLogo();

            string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")!;

            // https://github.com/dotnet/aspnetcore/blob/c925f99cddac0df90ed0bc4a07ecda6b054a0b02/src/DefaultBuilder/src/WebHost.cs#L169
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("Kaylumah_");

            if (args is { Length: > 0 })
            {
                configurationBuilder.AddCommandLine(args);
            }

            IConfiguration configuration = configurationBuilder.Build();
            IConfigurationRoot root = (IConfigurationRoot)configuration;
            string debugView = root.GetDebugView();
            Console.WriteLine(debugView);

            IServiceCollection services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                IConfigurationSection loggingSection = configuration.GetSection("Logging");
                builder.AddConfiguration(loggingSection);
                builder.AddSimpleConsole(opt =>
                {
                    opt.IncludeScopes = true;
                });
            });
            services.AddFileSystem();
            services.AddArtifactAccess(configuration);
            services.AddSiteManager(configuration);

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ISiteManager siteManager = serviceProvider.GetRequiredService<ISiteManager>();

            Stopwatch watch = new Stopwatch();
            Console.WriteLine("Start Site Generation");
            watch.Start();
            GenerateSiteRequest generateSiteRequest = new GenerateSiteRequest();
            await siteManager.GenerateSite(generateSiteRequest).ConfigureAwait(false);
            watch.Stop();
            Console.WriteLine($"Completed Site Generation in {watch.ElapsedMilliseconds} ms");
        }
    }
}
