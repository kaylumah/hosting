// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Kaylumah.Ssg.Engine.Transformation.Interface.Rendering;

public interface ISiteMetadata : IMetadata
{
    Dictionary<string, object> Data { get; set; }
    SortedDictionary<string, PageData[]> Collections { get; set; }
    SortedDictionary<string, PageData[]> Tags { get; set; }
    SortedDictionary<string, PageData[]> Series { get; set; }
    SortedDictionary<string, PageData[]> Types { get; set; }
    SortedDictionary<int, PageData[]> Years { get; set; }
}