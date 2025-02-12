﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public interface IFileProcessor
    {
        Task<IEnumerable<BinaryFile>> Process(FileFilterCriteria criteria);
    }

    public class FileFilterCriteria
    {
        public string RootDirectory
        { get; set; } = null!;
        public string[] DirectoriesToSkip
        { get; set; } = Array.Empty<string>();
        public string[] FileExtensionsToTarget
        { get; set; } = Array.Empty<string>();
    }
}
