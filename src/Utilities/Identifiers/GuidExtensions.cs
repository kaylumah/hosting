using Kaylumah.Ssg.Utilities;

namespace System
{
    public static class GuidUtilityExtensions
    {
        public static Guid CreateSiteGuid(this string site)
        {
            return GuidUtility.Create(GuidUtility.DnsNamespace, site, 5);
        }

        public static Guid CreatePageGuid(this Guid guid, string url)
        {
            return GuidUtility.Create(guid, url, 5);
        }
    }
}