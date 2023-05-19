﻿// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Kaylumah.Ssg.Access.Artifact.Interface;

[DebuggerDisplay("Artifact '{Path}'")]
public class Artifact
{
    public byte[] Contents { get; set; }
    public string Path { get; set; }
}
