﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO;
using System.Text;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    [DebuggerDisplay("File (Name={Name})")]
    public abstract class BinaryFile
    {
        public FileMetaData MetaData
        { get; set; }

        public byte[] Bytes
        { get; set; }

        public string Name => GetName();

        public BinaryFile(FileMetaData metadata, byte[] bytes)
        {
            MetaData = metadata;
            Bytes = bytes;
        }

        string GetName()
        {
            string fileName = Path.GetFileName(MetaData.Uri);
            return fileName;
        }
    }

    public class TextFile : BinaryFile
    {
        public string Content => GetContent();

        public string EncodingName
        { get; }

        public TextFile(FileMetaData metaData, byte[] bytes, string encodingName) : base(metaData, bytes)
        {
            EncodingName = encodingName;
        }

        string GetContent()
        {
            Encoding encoding = Encoding.GetEncoding(EncodingName);
            string content = encoding.GetString(Bytes);
            return content;
        }
    }
}
