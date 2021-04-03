// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Kaylumah.Ssg.Engine.Transformation.Interface;
using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service.Rendering
{
    public interface IPageMetadata : IMetadata
    {
        string Name { get;set; }
        string Collection { get;set; }
        List<string> Tags { get;set; }
    }
}