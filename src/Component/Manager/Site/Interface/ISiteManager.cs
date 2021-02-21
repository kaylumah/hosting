using System.Collections.Generic;
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
    public class SiteInfo
    {
        [DataMember]
        public Defaults[] Defaults { get; set; }
    }

    [DataContract]
    public class Defaults
    {
        [DataMember]
        public Scope Scope { get; set; }
        [DataMember]
        public Dictionary<string, object> Values { get; set; }
    }

    [DataContract]
    public class Scope
    {
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Type { get; set; }
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
