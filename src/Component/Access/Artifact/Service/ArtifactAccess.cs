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
            foreach(var artifact in request.Artifacts)
            {
                await File.WriteAllBytesAsync(artifact.Path, artifact.Contents);
            }
        }
    }
}
