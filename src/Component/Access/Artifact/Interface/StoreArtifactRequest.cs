// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Access.Artifact.Interface
{
    public class StoreArtifactsRequest
    {
        public Artifact[] Artifacts { get; set; }

        public OutputLocation OutputLocation { get; set; }

        public StoreArtifactsRequest(OutputLocation outputLocation, Artifact[] artifacts)
        {
            OutputLocation = outputLocation;
            Artifacts = artifacts;
        }
    }
}
