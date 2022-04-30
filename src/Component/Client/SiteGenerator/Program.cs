﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Kaylumah.Ssg.Access.Artifact.Hosting;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Access.Artifact.Service;
using Kaylumah.Ssg.Engine.Transformation.Hosting;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Engine.Transformation.Service.Plugins;
using Kaylumah.Ssg.Manager.Site.Hosting;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities;
using Kaylumah.Ssg.Utilities.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;

namespace Kaylumah.Ssg.Client.SiteGenerator;

class Program
{
    private static void ShowKaylumahLogo()
    {
        var applicationName = typeof(Program).Namespace;
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

        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        // https://github.com/dotnet/aspnetcore/blob/c925f99cddac0df90ed0bc4a07ecda6b054a0b02/src/DefaultBuilder/src/WebHost.cs#L169
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

        // todo UserSecrets?

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string> {
                    { $"{nameof(SiteConfiguration)}:Source", "_site" },
                    { $"{nameof(SiteConfiguration)}:Destination", "dist" },
                    { $"{nameof(SiteConfiguration)}:LayoutDirectory", "_layouts" },
                    { $"{nameof(SiteConfiguration)}:PartialsDirectory", "_includes" },
                    { $"{nameof(SiteConfiguration)}:DataDirectory", "_data" },
                    { $"{nameof(SiteConfiguration)}:AssetDirectory", "assets" }
            });

        // configurationBuilder.AddEnvironmentVariables();

        if (args != null)
        {
            configurationBuilder.AddCommandLine(args);
        }
        IConfiguration configuration = configurationBuilder.Build();
        var root = (IConfigurationRoot)configuration;
        var debugView = root.GetDebugView();
        Console.WriteLine(debugView);

        var rootDirectory = Path.Combine(Environment.CurrentDirectory, "_site");

        IServiceCollection services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddFileSystem(rootDirectory);
        services.AddArtifactAccess(configuration);
        services.AddTransformationEngine(configuration);
        services.AddSiteManager(configuration);
        

        var serviceProvider = services.BuildServiceProvider();
        var siteManager = serviceProvider.GetRequiredService<ISiteManager>();

        var siteConfiguration = new SiteConfiguration();
        configuration.GetSection(nameof(SiteConfiguration)).Bind(siteConfiguration);
        Stopwatch watch = new Stopwatch();
        Console.WriteLine("Start Site Generation");
        watch.Start();
        await siteManager.GenerateSite(new GenerateSiteRequest
        {
            Configuration = siteConfiguration
        });
        watch.Stop();
        Console.WriteLine($"Completed Site Generation in {watch.ElapsedMilliseconds} ms");
    }
}
