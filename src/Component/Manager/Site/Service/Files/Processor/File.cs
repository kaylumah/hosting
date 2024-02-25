﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    [DebuggerDisplay("File (Name={Name})")]
    public class TextFile
    {
        public FileMetaData MetaData
        { get; set; }
        public string Content
        { get; set; }
        public string Name => GetName();

        public TextFile(FileMetaData metaData, string content)
        {
            MetaData = metaData;
            Content = content;
        }

        string GetName()
        {
            string fileName = Path.GetFileName(MetaData.Uri);
            return fileName;
        }
    }
}
