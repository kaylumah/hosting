// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor;

public interface IFileProcessor
{
    Task<IEnumerable<File>> Process(FileFilterCriteria criteria);
}

public class FileFilterCriteria
{
    public string[] DirectoriesToSkip { get; set; }
    public string[] FileExtensionsToTarget { get; set; }
}