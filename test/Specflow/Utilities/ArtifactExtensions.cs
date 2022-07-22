﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Access.Artifact.Interface;

namespace Test.Specflow.Utilities;

public static class ArtifactExtensions
{
    public static byte[] GetArtifactContents(this IEnumerable<Artifact> artifacts, string path)
    {
        var bytes = artifacts.SingleOrDefault(x => path.Equals(x.Path))?.Contents ?? Array.Empty<byte>();
        return bytes;
    }
}
