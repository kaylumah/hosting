// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface ISiteMetadata
    {
        Dictionary<string, object> Data { get; set; }
        Dictionary<string, object> Collections { get; set; }
        Dictionary<string, object> Tags { get; set; }
    }
}