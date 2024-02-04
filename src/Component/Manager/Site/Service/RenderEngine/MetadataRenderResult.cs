// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine
{
    public class MetadataRenderResult
    {
        public string Content
        { get; set; }

        public MetadataRenderResult(string content)
        {
            Content = content;
        }
    }
}
