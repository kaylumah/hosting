

// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Hosting;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.DependencyInjection;
using Test.Utilities;

namespace Test.Specflow.Component.Access.Artifact
{
    public sealed class ArtifactAccessTestHarness
    {
        public TestHarnessBuilder TestHarnessBuilder { get; }

        readonly ValidationContext _ValidationContext;

        public ArtifactAccessTestHarness(MockFileSystem mockFileSystem, ValidationContext validationContext)
        {
            _ValidationContext = validationContext;
            TestHarnessBuilder = TestHarnessBuilder.Create()
                .Register((serviceCollection, configuration) =>
                {
                    serviceCollection.AddSingleton<IFileSystem>(mockFileSystem);
                    serviceCollection.AddArtifactAccess(configuration);
                });
        }

        public async Task TestArtifactAccess(Func<IArtifactAccess, Task> scenario)
        {
            TestHarness testHarness = TestHarnessBuilder.Build();
            await testHarness.TestService(scenario, _ValidationContext).ConfigureAwait(false);
        }
    }
}
