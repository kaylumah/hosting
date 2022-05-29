﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap;

public class SiteMap
{
    public IEnumerable<SiteMapNode> Items { get; set; }

    public SiteMapFormatter GetFormatter() => new SiteMapFormatter(this);

    public void SaveAsXml(XmlWriter writer)
    {
        GetFormatter().WriteXml(writer);
    }
}
