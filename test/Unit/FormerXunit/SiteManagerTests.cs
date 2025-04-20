﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Hosting;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Extensions.Data.Yaml;
using Kaylumah.Ssg.Manager.Site.Hosting;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class SiteManagerTests
    {
        [Fact]
        public async Task Test_SiteManager_GenerateSite()
        {
            Mock<IFileProcessor> fileProcessorMock = new Mock<IFileProcessor>();
            Mock<IArtifactAccess> artifactAccessMock = new Mock<IArtifactAccess>();
            MockFileSystem fileSystemMock = new MockFileSystem();
            fileSystemMock.Directory.CreateDirectory("_site");
            fileSystemMock.Directory.CreateDirectory(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceLayoutsDirectory);
            fileSystemMock.Directory.CreateDirectory(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourcePartialsDirectory);
            fileSystemMock.Directory.CreateDirectory(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceDataDirectory);
            fileSystemMock.Directory.CreateDirectory(Kaylumah.Ssg.Manager.Site.Service.Constants.Directories.SourceAssetsDirectory);

            Mock<IYamlParser> yamlParserMock = new Mock<IYamlParser>();
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            Dictionary<string, string?> siteConfig = new Dictionary<string, string?>
            {
                { "Site:Lang", null }
            };
            configurationBuilder.AddInMemoryCollection(siteConfig);
            Dictionary<string, string?> metaConfig = new Dictionary<string, string?>
            {
                { "Metadata:ExtensionMapping", null }
            };
            configurationBuilder.AddInMemoryCollection(metaConfig);

            IConfigurationRoot configuration = configurationBuilder.Build();
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddFileSystem()
                .AddArtifactAccess(configuration)
                .AddSiteManager(configuration)
                .AddSingleton(fileProcessorMock.Object)
                .AddSingleton(artifactAccessMock.Object)
                .AddSingleton<IFileSystem>(fileSystemMock)
                .AddSingleton(yamlParserMock.Object)
                .Configure<SiteInfo>(_ =>
                {
                    _.Url = "https://example.com";
                })
                .BuildServiceProvider();
            ISiteManager siteManager = serviceProvider.GetRequiredService<ISiteManager>();
            GenerateSiteRequest generateSiteRequest = new GenerateSiteRequest();
            await siteManager.GenerateSite(generateSiteRequest);
        }
    }
}
