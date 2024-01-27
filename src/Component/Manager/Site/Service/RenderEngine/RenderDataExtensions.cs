﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public static class RenderDataExtensions
    {
        public static readonly Func<RenderData, bool> Html = (renderData) => renderData.IsHtml();

        public static bool IsHtml(this RenderData renderData)
        {
            return PageMetaDataExtensions.Html(renderData.Page!);
        }
    }
}
