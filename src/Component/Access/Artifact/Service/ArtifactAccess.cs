// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Access.Artifact.Service
{
    public partial class ArtifactAccess : IArtifactAccess
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Information,
            Message = "Storing artifacts")]
        public partial void StoreArtifacts();

        readonly ILogger _Logger;
        readonly IEnumerable<IStoreArtifactsStrategy> _StoreArtifactsStrategies;

        public ArtifactAccess(ILogger<ArtifactAccess> logger, IEnumerable<IStoreArtifactsStrategy> storeArtifactsStrategies)
        {
            _Logger = logger;
            _StoreArtifactsStrategies = storeArtifactsStrategies;
        }

        public async Task Store(StoreArtifactsRequest request)
        {
            StoreArtifacts();
            IStoreArtifactsStrategy storeArtifactsStrategy = _StoreArtifactsStrategies.SingleOrDefault(strategy => strategy.ShouldExecute(request));
            await storeArtifactsStrategy.Execute(request).ConfigureAwait(false);
        }
    }
}
