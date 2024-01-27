// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public static class MetadataRenderRequestExtensions
    {
        public static readonly Func<MetadataRenderRequest, bool> Html = (metadataRenderRequest) => metadataRenderRequest.IsHtml();

        public static bool IsHtml(this MetadataRenderRequest request)
        {
            return RenderDataExtensions.Html(request.Metadata!);
        }
    }
}
