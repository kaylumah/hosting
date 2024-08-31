// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ServiceModel.Syndication;

namespace Kaylumah.Ssg.Manager.Site.Service.Feed
{
    public class FeedArtifact
    {
        public string FileName
        { get; set; }

        public SyndicationFeed SyndicationFeed
        { get; set; }

        public FeedArtifact(string fileName, SyndicationFeed syndicationFeed)
        {
            FileName = fileName;
            SyndicationFeed = syndicationFeed;
        }
    }
}