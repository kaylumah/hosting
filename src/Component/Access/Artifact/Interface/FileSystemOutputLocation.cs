// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
namespace Kaylumah.Ssg.Access.Artifact.Interface
{
    public class FileSystemOutputLocation : OutputLocation {
        public bool Clean { get; set; }
        public string Path { get; set; }
    }
}