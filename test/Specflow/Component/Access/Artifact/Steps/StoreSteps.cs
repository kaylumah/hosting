// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Access.Artifact.Interface;

namespace Test.Specflow.Component.Access.Artifact.Steps;

[Binding]
[Scope(Feature = "ArtifactAccess Store")]
public sealed class StoreSteps
{
    private readonly ArtifactAccessTestHarness _artifactAccessTestHarness;

    public StoreSteps(ArtifactAccessTestHarness artifactAccessTestHarness)
    {
        _artifactAccessTestHarness = artifactAccessTestHarness;
    }

    [When]
    public async Task WhenTheArtifactAccessIsCalled()
    {
        async Task TestScenario(IArtifactAccess artifactAccess)
        {
            await artifactAccess.Store(new StoreArtifactsRequest()
            {
                Artifacts = Array.Empty<Kaylumah.Ssg.Access.Artifact.Interface.Artifact>(),
                OutputLocation = new FileSystemOutputLocation() {}
            }).ConfigureAwait(false);
        }

        await _artifactAccessTestHarness.TestArtifactAccess(TestScenario).ConfigureAwait(false);
    }
}
