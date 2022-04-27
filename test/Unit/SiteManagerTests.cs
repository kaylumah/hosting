﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Microsoft.Extensions.Options;
using Moq;
using Test.Unit.Mocks;
using Xunit;

namespace Test.Unit;

public class SiteManagerTests
{
    [Fact]
    public async Task Test_SiteManager_GenerateSite()
    {
        var fileProcessorMock = new FileProcessorMock();
        var artifactAccessMock = new ArtifactAccessMock();
        var fileSystemMock = new FileSystemMock();
        var yamlParserMock = new YamlParserMock();
        var loggerMock = new LoggerMock<SiteManager>();
        var siteInfo = Options.Create(new SiteInfo
        {
            Url = "https://example.com"
        });
        var transformEngineMock = new Mock<ITransformationEngine>();
        var siteManager = new SiteManager(fileProcessorMock.Object, artifactAccessMock.Object, fileSystemMock.Object, yamlParserMock.Object, loggerMock.Object, siteInfo, transformEngineMock.Object);
        await siteManager.GenerateSite(new GenerateSiteRequest
        {
            Configuration = new SiteConfiguration
            {

            }
        });
    }
}