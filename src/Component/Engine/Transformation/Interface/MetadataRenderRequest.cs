// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
namespace Kaylumah.Ssg.Engine.Transformation.Interface;

public class MetadataRenderRequest
{
    public Rendering.RenderData Metadata { get; set; }
    public string Template { get; set; }
}