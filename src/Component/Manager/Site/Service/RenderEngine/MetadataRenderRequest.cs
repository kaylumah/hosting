// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public class MetadataRenderRequest
    {
        public RenderData Metadata
        { get; set; }
        public string Template
        { get; set; }

        public MetadataRenderRequest(RenderData metaData, string template)
        {
            Metadata = metaData;
            Template = template;
        }
    }
}
