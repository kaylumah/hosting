// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor
{
    public interface IFileProcessor
    {
        Task<IEnumerable<File>> Process(FileFilterCriteria criteria);
    }

    public class FileFilterCriteria
    {
        public string RootDirectory { get; set; } = string.Empty;
        public string[] DirectoriesToSkip { get; set; } = Array.Empty<string>();
        public string[] FileExtensionsToTarget { get; set; } = Array.Empty<string>();
    }
}
