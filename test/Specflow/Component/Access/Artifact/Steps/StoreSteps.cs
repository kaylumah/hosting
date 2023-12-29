// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using TechTalk.SpecFlow;

namespace Test.Specflow.Component.Access.Artifact.Steps
{
    [Binding]
    [Scope(Feature = "ArtifactAccess Store")]
    public sealed class StoreSteps
    {
        readonly ArtifactAccessTestHarness _ArtifactAccessTestHarness;

        public StoreSteps(ArtifactAccessTestHarness artifactAccessTestHarness)
        {
            _ArtifactAccessTestHarness = artifactAccessTestHarness;
        }

        [When]
        public async Task WhenTheArtifactAccessIsCalled()
        {
            async Task TestScenario(IArtifactAccess artifactAccess)
            {
                await artifactAccess.Store(new StoreArtifactsRequest()
                {
                    Artifacts = Array.Empty<Kaylumah.Ssg.Access.Artifact.Interface.Artifact>(),
                    OutputLocation = new FileSystemOutputLocation() { }
                }).ConfigureAwait(false);
            }

            await _ArtifactAccessTestHarness.TestArtifactAccess(TestScenario).ConfigureAwait(false);
        }
    }
}
