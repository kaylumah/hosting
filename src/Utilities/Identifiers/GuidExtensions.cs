// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Utilities;

namespace System;

public static class GuidUtilityExtensions
{
    public static Guid CreateSiteGuid(this string site)
    {
        return GuidUtility.Create(GuidUtility.DnsNamespace, site, 5);
    }

    public static Guid CreatePageGuid(this Guid siteGuid, string url)
    {
        return GuidUtility.Create(siteGuid, url, 5);
    }
}
