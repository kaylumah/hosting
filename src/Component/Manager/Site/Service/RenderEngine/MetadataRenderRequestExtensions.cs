﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public static class MetadataRenderRequestExtensions
    {
        public static readonly Func<MetadataRenderRequest, bool> Html;
        static MetadataRenderRequestExtensions()
        {
            Html = (metadataRenderRequest) => metadataRenderRequest.IsHtml();
        }
        public static bool IsHtml(this MetadataRenderRequest request)
        {
            bool result = RenderDataExtensions.Html(request.Metadata);
            return result;
        }
    }
}
