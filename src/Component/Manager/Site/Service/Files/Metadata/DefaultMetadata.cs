// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;

[DebuggerDisplay("DefaultMetadata Path '{Path}' Scope '{Scope}'")]
public class DefaultMetadata
{
    public string Path { get; set; }
    public string Scope { get; set; }
    public FileMetaData Values { get; set; }
}
