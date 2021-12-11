// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Kaylumah.Ssg.Engine.Transformation.Interface.Rendering;

public interface IPageMetadata : IMetadata
{
    string Name { get; set; }
    string Series { get; set; }
    string Collection { get; set; }
    List<string> Tags { get; set; }
    ContentType Type { get; set; }
}