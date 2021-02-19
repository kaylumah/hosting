using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    public interface ISiteManager
    {
        Task GenerateSite(GenerateSiteRequest request);
    }

    [DataContract]
    public class GenerateSiteRequest
    {
        [DataMember]
        public SiteConfiguration Configuration { get;set; }
    }

    [DataContract]
    public class SiteConfiguration
    {
        [DataMember]
        public string Source { get;set; }
        [DataMember]
        public string Destination { get;set; }
        [DataMember]
        public string LayoutDirectory { get;set; }
        [DataMember]
        public string PartialsDirectory { get;set; }
        [DataMember]
        public string DataDirectory { get;set; }
        [DataMember]
        public string AssetDirectory { get;set; }
    }
}
