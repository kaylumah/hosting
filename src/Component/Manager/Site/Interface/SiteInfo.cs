using System.Runtime.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    public class SiteInfo
    {
        public string Lang { get;set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Collections Collections { get; set; } = new Collections();
    }
}