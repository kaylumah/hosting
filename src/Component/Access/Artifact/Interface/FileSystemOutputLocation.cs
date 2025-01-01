﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Access.Artifact.Interface
{
    public class FileSystemOutputLocation : OutputLocation
    {
        public bool Clean
        { get; set; }
        public string Path
        { get; set; }

        public FileSystemOutputLocation(string path, bool clean)
        {
            Path = path;
            Clean = clean;
        }
    }
}
