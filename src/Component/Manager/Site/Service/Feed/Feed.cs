using System.ServiceModel.Syndication;

namespace Kaylumah.Ssg.Manager.Site.Service.Feed
{
    public class Feed
    {
        public string FileName
        { get; set; }

        public SyndicationFeed SyndicationFeed
        { get;set; }

        public Feed(string fileName, SyndicationFeed syndicationFeed)
        {
            FileName = fileName;
            SyndicationFeed = syndicationFeed;
        }
    }
}