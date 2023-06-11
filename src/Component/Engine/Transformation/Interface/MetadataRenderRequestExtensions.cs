// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

public static class MetadataRenderRequestExtensions
{
    public static readonly Func<MetadataRenderRequest, bool> Html = (metadataRenderRequest) => metadataRenderRequest.IsHtml();

    public static bool IsHtml(this MetadataRenderRequest request)
    {
        return RenderDataExtensions.Html(request.Metadata);
    }
}
