﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.iFX.Test;
using Moq;

namespace Test.Unit.Utilities
{
#pragma warning disable IDESIGN103
    public class ArtifactAccessMock : StrictMock<IArtifactAccess>
    {
        readonly List<StoreArtifactsRequest> _StoreArtifactsRequests;
        public ReadOnlyCollection<StoreArtifactsRequest> StoreArtifactRequests => new(_StoreArtifactsRequests);
        public ReadOnlyCollection<Artifact> Artifacts => new(StoreArtifactRequests.SelectMany(x => x.Artifacts).ToList());

        public ArtifactAccessMock()
        {
            _StoreArtifactsRequests = new();
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
