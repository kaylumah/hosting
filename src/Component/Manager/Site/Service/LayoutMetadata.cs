using YamlDotNet.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class LayoutMetadata
    {
        [YamlMember(Alias = "layout")]
        public string Layout { get; set; }
    }
}