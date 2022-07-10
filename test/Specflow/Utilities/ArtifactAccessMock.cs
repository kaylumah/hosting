// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Access.Artifact.Interface;
using Moq;

namespace Test.Specflow.Utilities;

public class ArtifactAccessMock : StrictMock<IArtifactAccess>
{
    public ArtifactAccessMock()
    {
        SetupStore();
    }

    public void SetupStore()
    {
        Setup(artifactAccess =>
                artifactAccess.Store(It.IsAny<StoreArtifactsRequest>()))
            .Returns(Task.CompletedTask);
    }
}
