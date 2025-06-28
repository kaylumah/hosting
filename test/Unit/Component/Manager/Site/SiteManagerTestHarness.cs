// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kaylumah.Ssg.iFX.Test;
using Kaylumah.Ssg.Manager.Site.Hosting;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using Reqnroll;
using Test.Unit.BDD;
using Test.Unit.Utilities;
using Test.Utilities;

namespace Test.Unit.Component.Manager.Site
{
    public sealed class SiteManagerTestHarness
    {
        public TestHarnessBuilder TestHarnessBuilder
        { get; }

        readonly ValidationContext _ValidationContext;

        public SiteManagerTestHarness(
            ArtifactAccessMock artifactAccessMock,
            MockFileSystem mockFileSystem,
            FakeTimeProvider fakeTimeProvider,
            MetadataParserOptions metadataParserOptions,
            SiteInfo siteInfo, ValidationContext validationContext)
        {
            _ValidationContext = validationContext;
            Dictionary<string, string?> config = new Dictionary<string, string?>()
            {
                { "Site", string.Empty },
                { "Metadata", string.Empty }
            };
            TestHarnessBuilder = TestHarnessBuilder.Create()
                .Configure(configurationBuilder =>
                {
                    configurationBuilder.AddInMemoryCollection(config);
                })
                .Register((services, configuration) =>
                {
                    services.AddSiteManager(configuration);
                    services.AddSingleton(artifactAccessMock.Object);
                    services.AddSingleton(metadataParserOptions);
                    services.AddSingleton(siteInfo);
                })
                .SetupTimeProvider(fakeTimeProvider)
                .SetupLogger()
                .SetupFileSystem(mockFileSystem);
        }

        public async Task TestSiteManager(Func<ISiteManager, Task> scenario)
        {
            TestHarness testHarness = TestHarnessBuilder.Build();
            await testHarness.TestService(scenario, _ValidationContext).ConfigureAwait(false);
        }
    }
}
