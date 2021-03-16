// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Access.Artifact.Interface
{
    public class Artifact
    {
        public byte[] Contents { get; set; }
        public string Path { get; set; }
    }

    public class StoreArtifactsRequest
    {
        public Artifact[] Artifacts { get; set; }
    }

    public interface IArtifactAccess
    {
        Task Store(StoreArtifactsRequest request);
    }
}
