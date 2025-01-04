// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Reqnroll;

namespace Test.Unit.Component.Access.Artifact.Steps
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
            static async Task TestScenario(IArtifactAccess artifactAccess)
            {
                FileSystemOutputLocation fileSystemOutputLocation = new FileSystemOutputLocation(string.Empty, false);
                Kaylumah.Ssg.Access.Artifact.Interface.Artifact[] artifacts = Array.Empty<Kaylumah.Ssg.Access.Artifact.Interface.Artifact>();
                StoreArtifactsRequest storeArtifactsRequest = new StoreArtifactsRequest(fileSystemOutputLocation, artifacts);
                await artifactAccess.Store(storeArtifactsRequest).ConfigureAwait(false);
            }

            await _ArtifactAccessTestHarness.TestArtifactAccess(TestScenario).ConfigureAwait(false);
        }
    }
}
