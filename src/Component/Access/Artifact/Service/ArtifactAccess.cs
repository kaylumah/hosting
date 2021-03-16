// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;

namespace Kaylumah.Ssg.Access.Artifact.Service
{
    public class ArtifactAccess : IArtifactAccess
    {
        public async Task Store(StoreArtifactsRequest request)
        {
            foreach (var artifact in request.Artifacts)
            {
                var index = artifact.Path.LastIndexOf(Path.DirectorySeparatorChar);
                var artifactDirectory = artifact.Path.Substring(0, index);
                if (!Directory.Exists(artifactDirectory))
                {
                    Directory.CreateDirectory(artifactDirectory);
                }
                await File.WriteAllBytesAsync(artifact.Path, artifact.Contents);
            }
        }
    }
}
