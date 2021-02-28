using System.Runtime.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    [DataContract]
    public class SiteInfo
    {
        [DataMember]
        public Collections Collections { get; set; } = new Collections();
    }
}