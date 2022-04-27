// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Access.Artifact.Interface;

public interface IArtifactAccess
{
    Task Store(StoreArtifactsRequest request);
}