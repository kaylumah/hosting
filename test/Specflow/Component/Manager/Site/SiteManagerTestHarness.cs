
// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kaylumah.Ssg.Manager.Site.Hosting;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow.Infrastructure;
using Test.Specflow.Utilities;
using Test.Utilities;

namespace Test.Specflow.Component.Manager.Site
{
    public sealed class SiteManagerTestHarness
    {
        public TestHarnessBuilder TestHarnessBuilder { get; }

        readonly ValidationContext _ValidationContext;

        public SiteManagerTestHarness(
            ISpecFlowOutputHelper specFlowOutputHelper,
            ArtifactAccessMock artifactAccessMock,
            MockFileSystem mockFileSystem,
            MetadataParserOptions metadataParserOptions,
            SystemClockMock systemClockMock,
            SiteInfo siteInfo, ValidationContext validationContext)
        {
            _ValidationContext = validationContext;
            TestHarnessBuilder = TestHarnessBuilder.Create()
                .Configure(configurationBuilder =>
                {
                    configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Site"] = string.Empty,
                        ["Metadata"] = string.Empty
                    });
                })
                .Register(services =>
                {
                    services.AddSingleton<IAsyncInterceptor>(new MyInterceptor(specFlowOutputHelper));
                })
                .Register((services, configuration) =>
                {
                    services.AddSiteManager(configuration);
                    services.AddSingleton(systemClockMock.Object);
                    services.AddSingleton(artifactAccessMock.Object);
                    services.AddSingleton<IFileSystem>(mockFileSystem);
                    services.AddSingleton(metadataParserOptions);
                    services.AddSingleton(siteInfo);
                });
        }

        public async Task TestSiteManager(Func<ISiteManager, Task> scenario)
        {
            TestHarness testHarness = TestHarnessBuilder.Build();
            await testHarness.TestService(scenario, _ValidationContext).ConfigureAwait(false);
        }
    }
}
