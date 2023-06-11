// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;

namespace Kaylumah.Ssg.Manager.Site.Service.SiteMap;

public class SiteMapNode
{
    public SitemapFrequency? Frequency { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public double? Priority { get; set; }
    public string Url { get; set; }
}
