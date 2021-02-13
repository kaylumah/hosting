using System;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;

namespace Kaylumah.Ssg.Access.Artifact.Service
{
    public class ArtifactAccess : IArtifactAccess
    {
        public Task Store(StoreArtifactsRequest request)
        {
            return Task.CompletedTask;
        }
    }
}
