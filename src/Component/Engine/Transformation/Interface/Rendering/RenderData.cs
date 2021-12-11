// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
namespace Kaylumah.Ssg.Engine.Transformation.Interface.Rendering;

public class RenderData : IMetadata
{
    public BuildData Build { get; set; }
    public ISiteMetadata Site { get; set; }
    public IPageMetadata Page { get; set; }
    public string Content => Page?.Content ?? string.Empty;
    public string Title => Page?.Title ?? Site?.Title ?? null;
    public string Description => Page?.Description ?? Site?.Description ?? null;
    public string Language => Page?.Language ?? Site?.Language ?? null;
    public string Author => Page?.Author ?? Site?.Author ?? null;
    public string Url => Page?.Url ?? Site?.Url ?? null;

}