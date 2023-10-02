// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Utilities;

namespace System;

public static class GuidUtilityExtensions
{
    public static Guid CreateSiteGuid(this string site)
    {
        var result = GuidUtility.Create(GuidUtility.DnsNamespace, site, 5);
        return result;
    }

    public static Guid CreatePageGuid(this Guid siteGuid, string url)
    {
        var result = GuidUtility.Create(siteGuid, url, 5);
        return result;
    }
}
