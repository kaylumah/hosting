// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.Seo;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Microsoft.Extensions.Logging.Abstractions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Test.Specflow.Utilities;
using Test.Utilities;

namespace Test.Specflow;

public sealed class SiteManagerTestHarness
{
    public TestHarnessBuilder TestHarnessBuilder { get; }
    private readonly ISiteManager _siteManager;

    public SiteManagerTestHarness(
        ArtifactAccessMock artifactAccessMock,
        MockFileSystem mockFileSystem,
        MetadataParserOptions metadataParserOptions,
        SystemClockMock systemClockMock,
        SiteInfo siteInfo)
    {
        TestHarnessBuilder = TestHarnessBuilder.Create();
        IMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(new YamlParser());
        ITransformationEngine transformationEngine = new TransformationEngine(
            NullLogger<TransformationEngine>.Instance,
            mockFileSystem,
            metadataProvider);
        var metadataParser = new FileMetadataParser(NullLogger<FileMetadataParser>.Instance,
            new YamlFrontMatterMetadataProvider(new YamlParser()),
            metadataParserOptions);
        var fileProcessor = new FileProcessor(mockFileSystem,
            NullLogger<FileProcessor>.Instance,
            Enumerable.Empty<IContentPreprocessorStrategy>(),
            siteInfo,
            metadataParser);
        var logger = NullLogger<SiteManager>.Instance;
        var yamlParser = new YamlParser();
        var siteMetadataFactory = new SiteMetadataFactory(systemClockMock.Object, siteInfo, yamlParser, mockFileSystem,
            NullLogger<SiteMetadataFactory>.Instance);
        var feedGenerator = new FeedGenerator(NullLogger<FeedGenerator>.Instance);
        var metaTagGenerator = new MetaTagGenerator(NullLogger<MetaTagGenerator>.Instance);
        var structureDataGenerator = new StructureDataGenerator(NullLogger<StructureDataGenerator>.Instance);
        var seoGenerator = new SeoGenerator(metaTagGenerator, structureDataGenerator);
        var siteMapGenerator = new SiteMapGenerator(NullLogger<SiteMapGenerator>.Instance);
        
        _siteManager = new SiteManager(
            fileProcessor,
            artifactAccessMock.Object,
            mockFileSystem,
            logger,
            siteInfo,
            transformationEngine,
            siteMetadataFactory,
            feedGenerator,
            seoGenerator,
            siteMapGenerator,
            systemClockMock.Object);
    }

    public async Task TestSiteManager(Func<ISiteManager, Task> scenario)
    {
        var testHarness = TestHarnessBuilder.Build();

        await testHarness.TestService(scenario).ConfigureAwait(false);
        // await scenario(_siteManager).ConfigureAwait(false);
    }
}
