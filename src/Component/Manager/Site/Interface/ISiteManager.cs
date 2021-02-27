using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    public interface ISiteManager
    {
        Task GenerateSite(GenerateSiteRequest request);
    }

    public class Collection
    {
        public string Name { get;set; }
        public bool Output { get;set; }
    }

    public class Collections : KeyedCollection<string, Collection>
    {
        protected override string GetKeyForItem(Collection item) => item.Name;
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
        public Dictionary<string, string> ExtensionMapping { get;set; }

        [DataMember]
        public Defaults[] Defaults { get; set; }

        [DataMember]
        public Collections Collections { get;set; }
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
