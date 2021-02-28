using System.Runtime.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    [DataContract]
    public class GenerateSiteRequest
    {
        [DataMember]
        public SiteConfiguration Configuration { get; set; }
    }
}