// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
namespace Kaylumah.Ssg.Utilities
{
    public class File<TMetadata>
    {
        public string Encoding { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
        public TMetadata Data { get; set; }
    }
}