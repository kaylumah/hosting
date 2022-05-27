﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor;

[DebuggerDisplay("{Name} {Files.Length} Files")]
public class FileCollection
{
    public string Name { get; set; }
    public File[] Files { get; set; }
}
