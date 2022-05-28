// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Engine.Transformation.Interface;
public class MetadataRenderRequest
{
    public static readonly Func<MetadataRenderRequest, bool> IsHtml = metadataRenderRequest => RenderData.IsHtml(metadataRenderRequest.Metadata);
    public RenderData Metadata { get; set; }
    public string Template { get; set; }
}
