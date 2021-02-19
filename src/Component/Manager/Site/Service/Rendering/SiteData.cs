using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    class SiteData
    {
        public string Title { get;set; } = $"{nameof(SiteData)}{nameof(Title)}";
        public Dictionary<string, object> Data { get;set; }
        public Dictionary<string, object> Collections { get;set; }
    }
}