using System.Collections.ObjectModel;

namespace Kaylumah.Ssg.Manager.Site.Interface
{
    public class Collections : KeyedCollection<string, Collection>
    {
        protected override string GetKeyForItem(Collection item) => item.Name;
    }
}