// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Kaylumah.Ssg.Access.Artifact.Interface;

public interface IArtifactAccess
{
    Task Store(StoreArtifactsRequest request);
}
