// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Engine.Transformation.Interface;
using System.Collections.Generic;

namespace Kaylumah.Ssg.Engine.Transformation.Interface.Rendering
{
    public interface ISiteMetadata : IMetadata
    {
        Dictionary<string, object> Data { get; set; }
        SortedDictionary<string, PageData[]> Collections { get; set; }
        SortedDictionary<string, PageData[]> Tags { get; set; }
    }
}