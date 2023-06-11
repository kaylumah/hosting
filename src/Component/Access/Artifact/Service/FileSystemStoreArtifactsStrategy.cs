﻿// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Access.Artifact.Service;

public partial class FileSystemStoreArtifactsStrategy : IStoreArtifactsStrategy
{
    [LoggerMessage(
    EventId = 0,
    Level = LogLevel.Information,
    Message = "Creating directory `{DirectoryName}`")]
    public partial void CreatingDirectory(string directoryName);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Creating file `{FileName}`")]
    public partial void CreatingFile(string fileName);
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public FileSystemStoreArtifactsStrategy(ILogger<FileSystemStoreArtifactsStrategy> logger, IFileSystem fileSystem)
    {
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public async Task Execute(StoreArtifactsRequest request)
    {
        if (request.OutputLocation is FileSystemOutputLocation fileSystemOutputLocation)
        {
            foreach (var artifact in request.Artifacts)
            {
                var filePath = Path.Combine(fileSystemOutputLocation.Path, artifact.Path);
                var directory = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directory))
                {
                    CreatingDirectory(directory);
                    _fileSystem.Directory.CreateDirectory(directory);
                }

                CreatingFile(filePath);
                await _fileSystem.File.WriteAllBytesAsync(filePath, artifact.Contents).ConfigureAwait(false);
            }
        }
    }

    public bool ShouldExecute(StoreArtifactsRequest request) => request.OutputLocation is FileSystemOutputLocation;
}
