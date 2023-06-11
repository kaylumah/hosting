// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
