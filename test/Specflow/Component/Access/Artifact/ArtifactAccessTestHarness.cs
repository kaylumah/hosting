// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Access.Artifact.Interface;
using Test.Utilities;

namespace Test.Specflow.Component.Access.Artifact;

public sealed class ArtifactAccessTestHarness
{
    public TestHarnessBuilder TestHarnessBuilder { get; }

    public ArtifactAccessTestHarness()
    {
        TestHarnessBuilder = TestHarnessBuilder.Create()
            .Register((serviceCollection, configuration) => { });
    }
    
    public async Task TestArtifactAccess(Func<IArtifactAccess, Task> scenario)
    {
        var testHarness = TestHarnessBuilder.Build();
        await testHarness.TestService(scenario).ConfigureAwait(false);
    }
}
