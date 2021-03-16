using System.Runtime.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    [DataContract]
    public class SiteConfiguration
    {
        [DataMember]
        public string Source { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public string LayoutDirectory { get; set; }
        [DataMember]
        public string PartialsDirectory { get; set; }
        [DataMember]
        public string DataDirectory { get; set; }
        [DataMember]
        public string AssetDirectory { get; set; }
    }
}