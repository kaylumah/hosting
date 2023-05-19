// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Castle.DynamicProxy;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.Seo;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using TechTalk.SpecFlow.Infrastructure;
using Test.Specflow.Utilities;
using Test.Utilities;

namespace Test.Specflow.Component.Manager.Site;

public sealed class SiteManagerTestHarness
{
    public TestHarnessBuilder TestHarnessBuilder { get; }

    public SiteManagerTestHarness(
        ISpecFlowOutputHelper specFlowOutputHelper,
        ArtifactAccessMock artifactAccessMock,
        MockFileSystem mockFileSystem,
        MetadataParserOptions metadataParserOptions,
        SystemClockMock systemClockMock,
        SiteInfo siteInfo)
    {
        TestHarnessBuilder = TestHarnessBuilder.Create()
            .Register(services =>
            {
                services.AddSingleton<IAsyncInterceptor>(new MyInterceptor(specFlowOutputHelper));
            })
            .Register(services =>
            {
                services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));
                services.AddSingleton(artifactAccessMock.Object);
                services.AddSingleton(systemClockMock.Object);
                services.AddSingleton<IFileSystem>(mockFileSystem);
                services.AddSingleton<IYamlParser, YamlParser>();
                services.AddSingleton<IMetadataProvider, YamlFrontMatterMetadataProvider>();
                services.AddSingleton<ITransformationEngine, TransformationEngine>();
                services.AddSingleton<IFileMetadataParser, FileMetadataParser>();
                services.AddSingleton(metadataParserOptions);
                services.AddSingleton<IFileProcessor, FileProcessor>();
                services.AddSingleton(siteInfo);
                services.AddSingleton<SiteMetadataFactory>();
                services.AddSingleton<FeedGenerator>();
                services.AddSingleton<MetaTagGenerator>();
                services.AddSingleton<StructureDataGenerator>();
                services.AddSingleton<SeoGenerator>();
                services.AddSingleton<SiteMapGenerator>();
                services.AddSingleton<ISiteManager, SiteManager>();
            });
    }

    public async Task TestSiteManager(Func<ISiteManager, Task> scenario)
    {
        var testHarness = TestHarnessBuilder.Build();
        await testHarness.TestService(scenario).ConfigureAwait(false);
    }
}
