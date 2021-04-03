// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Access.Artifact.Service
{
    public class FileSystemStoreArtifactsStrategy : IStoreArtifactsStrategy
    {
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
                // TODO look at clean

                foreach (var artifact in request.Artifacts)
                {
                    var filePath = Path.Combine(fileSystemOutputLocation.Path, artifact.Path);
                    var directory = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    //_logger.LogDebug($"Writing file: {filePath}");
                    //_logger.LogDebug($"{file.Contents}");

                    await File.WriteAllBytesAsync(filePath, artifact.Contents).ConfigureAwait(false);
                }


                return;
            }
            throw new InvalidOperationException();
            // foreach (var artifact in request.Artifacts)
            // {
            //     var index = artifact.Path.LastIndexOf(Path.DirectorySeparatorChar);
            //     var artifactDirectory = artifact.Path.Substring(0, index);
            //     if (!Directory.Exists(artifactDirectory))
            //     {
            //         Directory.CreateDirectory(artifactDirectory);
            //     }
            //     await File.WriteAllBytesAsync(artifact.Path, artifact.Contents);
            // }
        }

        public bool ShouldExecute(StoreArtifactsRequest request) => request.OutputLocation is FileSystemOutputLocation;
    }
}