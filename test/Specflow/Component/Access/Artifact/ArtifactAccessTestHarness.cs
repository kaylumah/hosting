// Copyright (c) Kaylumah, 2023. All rights reserved.
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

        private readonly ValidationContext _validationContext;

        public ArtifactAccessTestHarness(MockFileSystem mockFileSystem, ValidationContext validationContext)
        {
            _validationContext = validationContext;
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
            await testHarness.TestService(scenario, _validationContext).ConfigureAwait(false);
        }
    }
}
