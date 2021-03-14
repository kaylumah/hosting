using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class MetadataParserOptions
    {
        public const string Options = "Metadata";
        public DefaultMetadatas Defaults { get; set; } = new DefaultMetadatas();
        public Dictionary<string, string> ExtensionMapping { get; set; } = new Dictionary<string, string>();
    }
}