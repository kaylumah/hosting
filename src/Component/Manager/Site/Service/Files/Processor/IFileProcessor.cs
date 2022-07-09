// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Processor;

public interface IFileProcessor
{
    Task<IEnumerable<File>> Process(FileFilterCriteria criteria);
}

public class FileFilterCriteria
{
    public string RootDirectory { get;set; }
    public string[] DirectoriesToSkip { get; set; } = Array.Empty<string>();
    public string[] FileExtensionsToTarget { get; set; } = Array.Empty<string>();
}
