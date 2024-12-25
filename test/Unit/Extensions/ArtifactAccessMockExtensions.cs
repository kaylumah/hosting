// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using HtmlAgilityPack;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;

namespace Test.Unit.Utilities
{
    public static class ArtifactAccessMockExtensions
    {
        public static string GetString(this ArtifactAccessMock artifactAccess, string path) => artifactAccess.GetArtifactContents(path).GetString();

        public static byte[] GetArtifactContents(this ArtifactAccessMock artifactAccess, string path)
        {
            byte[] bytes = artifactAccess.Artifacts.GetArtifactContents(path);
            return bytes;
        }

        public static HtmlDocument GetHtmlDocument(this ArtifactAccessMock artifactAccess, string path)
            => artifactAccess.GetArtifactContents(path).ToHtmlDocument();

        public static FeedArtifact GetFeedArtifact(this ArtifactAccessMock artifactAccess, string path = "feed.xml")
            => artifactAccess.GetArtifactContents(path).ToSyndicationFeed(path);

        public static SiteMapArtifact GetSiteMapArtifact(this ArtifactAccessMock artifactAccess, string path = "sitemap.xml")
            => artifactAccess.GetArtifactContents(path).ToSiteMap(path);
    }
}
