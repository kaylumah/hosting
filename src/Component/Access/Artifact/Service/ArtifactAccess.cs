// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Access.Artifact.Interface;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Access.Artifact.Service;

public partial class ArtifactAccess : IArtifactAccess
{

    private readonly ILogger _logger;
    private readonly IEnumerable<IStoreArtifactsStrategy> _storeArtifactsStrategies;

    public ArtifactAccess(ILogger<ArtifactAccess> logger, IEnumerable<IStoreArtifactsStrategy> storeArtifactsStrategies)
    {
        _logger = logger;
        _storeArtifactsStrategies = storeArtifactsStrategies;
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Storing artifacts")]
    public partial void StoreArtifacts();

    public async Task Store(StoreArtifactsRequest request)
    {
        StoreArtifacts();
        var storeArtifactsStrategy = _storeArtifactsStrategies.SingleOrDefault(strategy => strategy.ShouldExecute(request));
        await storeArtifactsStrategy.Execute(request).ConfigureAwait(false);
    }
}
