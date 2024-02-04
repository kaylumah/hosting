// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Utilities
{
    public class File<TMetadata>
    {
        public string Encoding
        { get; set; }
        public string Name => GetFileName();
        public string Path
        { get; set; }
        public string Content
        { get; set; }
        public TMetadata Data
        { get; set; }

        public File(string path, string content, TMetadata metadata, string encoding)
        {
            Path = path;
            Content = content;
            Data = metadata;
            Encoding = encoding;
        }

        internal string GetFileName()
        {
            string fileName = System.IO.Path.GetFileName(Path);
            return fileName;
        }
    }
}
