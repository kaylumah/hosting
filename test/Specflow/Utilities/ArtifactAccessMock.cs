// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Moq;

namespace Test.Specflow.Utilities
{
    public class ArtifactAccessMock : StrictMock<IArtifactAccess>
    {
        readonly List<StoreArtifactsRequest> _StoreArtifactsRequests = new();
        public ReadOnlyCollection<StoreArtifactsRequest> StoreArtifactRequests => new(_StoreArtifactsRequests);
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
                    _StoreArtifactsRequests.Add(request);
                })
                .Returns(Task.CompletedTask);
        }
    }
}
