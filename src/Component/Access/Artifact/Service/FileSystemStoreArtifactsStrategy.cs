// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Access.Artifact.Service
{
    public partial class FileSystemStoreArtifactsStrategy : IStoreArtifactsStrategy
    {
        [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Trace,
        Message = "Creating directory `{DirectoryName}`")]
        public partial void CreatingDirectory(string directoryName);

        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Trace,
            Message = "Creating file `{FileName}`")]
        public partial void CreatingFile(string fileName);

        readonly IFileSystem _FileSystem;
        readonly ILogger _Logger;

        public FileSystemStoreArtifactsStrategy(ILogger<FileSystemStoreArtifactsStrategy> logger, IFileSystem fileSystem)
        {
            _Logger = logger;
            _FileSystem = fileSystem;
        }

        public async Task Execute(StoreArtifactsRequest request)
        {
            if (request.OutputLocation is FileSystemOutputLocation fileSystemOutputLocation)
            {
                foreach (Interface.Artifact artifact in request.Artifacts)
                {
                    string filePath = Path.Combine(fileSystemOutputLocation.Path, artifact.Path);
                    string directory = Path.GetDirectoryName(filePath)!;

                    if (!Directory.Exists(directory))
                    {
                        CreatingDirectory(directory);
                        _FileSystem.Directory.CreateDirectory(directory);
                    }

                    CreatingFile(filePath);
                    await _FileSystem.File.WriteAllBytesAsync(filePath, artifact.Contents).ConfigureAwait(false);
                }
            }
        }

        public bool ShouldExecute(StoreArtifactsRequest request) => request.OutputLocation is FileSystemOutputLocation;
    }
}
