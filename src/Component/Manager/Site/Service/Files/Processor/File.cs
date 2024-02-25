// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    [DebuggerDisplay("File (Name={Name})")]
    public abstract class File
    {
        public FileMetaData MetaData
        { get; set; }

        public string Name => GetName();

        public File(FileMetaData metadata)
        {
            MetaData = metadata;
        }

        string GetName()
        {
            string fileName = Path.GetFileName(MetaData.Uri);
            return fileName;
        }
    }

    public class TextFile : File
    {
        public string Content
        { get; set; }

        public TextFile(FileMetaData metaData, string content) : base(metaData)
        {
            Content = content;
        }
    }
}
