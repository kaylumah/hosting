// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Engine.Transformation.Interface;
using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service.Rendering
{
    public interface ISiteMetadata : IMetadata
    {
        Dictionary<string, object> Data { get; set; }
        Dictionary<string, PageData[]> Collections { get; set; }
        Dictionary<string, PageData[]> Tags { get; set; }
    }
}