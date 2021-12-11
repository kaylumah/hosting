// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Access.Artifact.Interface;

public interface IArtifactAccess
{
    Task Store(StoreArtifactsRequest request);
}