// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Moq;

namespace Test.Specflow.Utilities;

public class ArtifactAccessMock : StrictMock<IArtifactAccess>
{
    private readonly List<StoreArtifactsRequest> _storeArtifactsRequests = new();
    public ReadOnlyCollection<StoreArtifactsRequest> StoreArtifactRequests => new(_storeArtifactsRequests);
    public ReadOnlyCollection<Artifact> Artifacts => new(StoreArtifactRequests.SelectMany(x => x.Artifacts).ToList());

    public ArtifactAccessMock()
    {
        SetupStore();
    }

    public void SetupStore()
    {
        Setup(artifactAccess =>
                artifactAccess.Store(It.IsAny<StoreArtifactsRequest>()))
            .Callback((StoreArtifactsRequest request) =>
            {
                _storeArtifactsRequests.Add(request);
            })
            .Returns(Task.CompletedTask);
    }
}
