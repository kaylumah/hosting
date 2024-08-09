namespace Kaylumah.Ssg.Manager.Site.Service.Search
{
    public class SearchIndex
    {
        public IndexItem[] IndexItems
        { get;set; }

        public SearchIndex(IndexItem[] indexItems)
        {
            IndexItems =  indexItems;
        }
    }
}