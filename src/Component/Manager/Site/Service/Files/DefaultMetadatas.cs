using System.Collections.ObjectModel;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class DefaultMetadatas : KeyedCollection<string, DefaultMetadata>
    {
        protected override string GetKeyForItem(DefaultMetadata item)
        {
            return item.Path;
        }
    }
}