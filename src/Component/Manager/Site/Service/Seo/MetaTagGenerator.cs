// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo;

public partial class MetaTagGenerator
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Attempting MetaTags `{Path}`")]
    private partial void LogMetaTags(string path);

    private readonly ILogger _logger;

    public MetaTagGenerator(ILogger<MetaTagGenerator> logger)
    {
        _logger = logger;
    }

    public string ToMetaTags(RenderData renderData)
    {
        ArgumentNullException.ThrowIfNull(renderData);
        LogMetaTags(renderData.Page.Uri);
        return string.Empty;
    }
}
