﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Syndication;
using HtmlAgilityPack;
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

        public static SyndicationFeed GetFeedArtifact(this ArtifactAccessMock artifactAccess, string path = "feed.xml")
            => artifactAccess.GetArtifactContents(path).ToSyndicationFeed();

        public static SiteMap GetSiteMapArtifact(this ArtifactAccessMock artifactAccess, string path = "sitemap.xml")
            => artifactAccess.GetArtifactContents(path).ToSiteMap();
    }
}
