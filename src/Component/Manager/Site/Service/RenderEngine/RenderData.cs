
// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public class RenderData
    {
        public SiteMetaData Site { get; set; }
        public PageMetaData Page { get; set; }
        public string Content => Page?.Content ?? string.Empty;
        public string Title => Page?.Title ?? Site?.Title ?? null;
        public string Description => Page?.Description ?? Site?.Description ?? null;
        public string Language => Page?.Language ?? Site?.Language ?? null;
        public string Author => Page?.Author ?? Site?.Author ?? null;
        public string Url => Page?.Uri ?? Site?.Url ?? null;

    }
}
