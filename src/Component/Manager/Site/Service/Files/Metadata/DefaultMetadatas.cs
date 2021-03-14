using System.Collections.ObjectModel;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class DefaultMetadatas : KeyedCollection<string, DefaultMetadata>
    {
        protected override string GetKeyForItem(DefaultMetadata item)
        {
            if (item.Scope != null)
            {
                return $"{item.Path}.{item.Scope}";
            }
            return item.Path;
        }
    }
}