// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.Seo;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Kaylumah.Ssg.Utilities;
using Kaylumah.Ssg.Utilities.Time;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ssg.Extensions.Data.Yaml;

namespace Test.Specflow.Steps;

[Binding]
[Scope(Feature = "SiteManager")]
public class SiteManagerStepDefinitions
{
    private readonly ISiteManager _siteManager;

    public SiteManagerStepDefinitions(SiteInfo siteInfo)
    {
        var clock = new Mock<ISystemClock>();
        var fileProcessor = new Mock<IFileProcessor>();
        var artifactAccess = new Mock<IArtifactAccess>();
        var fileSystem = new Mock<IFileSystem>();
        var logger = NullLogger<SiteManager>.Instance;
        var transformationEngine = new Mock<ITransformationEngine>();
        var yamlParser = new YamlParser();
        var siteMetadataFactory = new SiteMetadataFactory(clock.Object, siteInfo, yamlParser, fileSystem.Object, NullLogger<SiteMetadataFactory>.Instance);
        var feedGenerator = new FeedGenerator(NullLogger<FeedGenerator>.Instance);
        var metaTagGenerator = new MetaTagGenerator(NullLogger<MetaTagGenerator>.Instance);
        var structureDataGenerator = new StructureDataGenerator(NullLogger<StructureDataGenerator>.Instance);
        var seoGenerator = new SeoGenerator(metaTagGenerator, structureDataGenerator);
        var siteMapGenerator = new SiteMapGenerator(NullLogger<SiteMapGenerator>.Instance);
        _siteManager = new SiteManager(
            fileProcessor.Object,
            artifactAccess.Object,
            fileSystem.Object,
            logger,
            siteInfo,
            transformationEngine.Object,
            siteMetadataFactory,
            feedGenerator,
            seoGenerator,
            siteMapGenerator,
            clock.Object);
    }

    [When("the site is generated:")]
    public async Task WhenTheSiteIsGenerated()
    {
        try
        {
            await _siteManager.GenerateSite(new GenerateSiteRequest()
            {
                Configuration = new SiteConfiguration()
                {
                    AssetDirectory = "assets",
                    DataDirectory = "data"
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
