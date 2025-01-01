﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata
{
    public class DefaultMetadata
    {
        public string Path
        { get; set; } = null!;
        public string Scope
        { get; set; } = null!;
        public string[] Extensions
        { get; set; } = null!;
        public FileMetaData Values
        { get; set; } = null!;
    }
}
