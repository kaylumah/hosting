// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Access.Artifact.Service
{

    public interface IStoreArtifactsStrategy
    {
        Task Execute(StoreArtifactsRequest request);
        bool ShouldExecute(StoreArtifactsRequest request);
    }

    public class FileSystemStoreArtifactsStrategy : IStoreArtifactsStrategy
    {
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

    public class ArtifactAccess : IArtifactAccess
    {

        private readonly ILogger _logger;
        private readonly IEnumerable<IStoreArtifactsStrategy> _storeArtifactsStrategies;

        public ArtifactAccess(ILogger<ArtifactAccess> logger, IEnumerable<IStoreArtifactsStrategy> storeArtifactsStrategies)
        {
            _logger = logger;
            _storeArtifactsStrategies = storeArtifactsStrategies;
        }

        public async Task Store(StoreArtifactsRequest request)
        {
            var storeArtifactsStrategy = _storeArtifactsStrategies.SingleOrDefault(strategy => strategy.ShouldExecute(request));
            await storeArtifactsStrategy.Execute(request);
        }
    }
}
