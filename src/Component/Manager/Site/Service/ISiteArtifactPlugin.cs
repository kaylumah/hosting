// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface ISiteArtifactPlugin
    {
        Artifact[] Generate(SiteMetaData siteMetaData);
    }

    public class SiteMapSiteArtifactPlugin : ISiteArtifactPlugin
    {
        readonly SiteMapGenerator _SiteMapGenerator;

        public SiteMapSiteArtifactPlugin(SiteMapGenerator siteMapGenerator)
        {
            _SiteMapGenerator = siteMapGenerator;
        }

        public Artifact[] Generate(SiteMetaData siteMetaData)
        {
            List<SiteMap.SiteMap> siteMaps = new List<SiteMap.SiteMap>();
            // TODO: Consider the split
            SiteMap.SiteMap generatedSiteMap = _SiteMapGenerator.Create(siteMetaData);
            siteMaps.Add(generatedSiteMap);

            List<SiteMapIndexNode> siteMapIndexNodes = new List<SiteMapIndexNode>();
            List<Artifact> siteMapArtifacts = new List<Artifact>();
            foreach(SiteMap.SiteMap siteMap in siteMaps)
            {
                SiteMapIndexNode siteMapIndexNode = new SiteMapIndexNode();
                siteMapIndexNode.Url = siteMap.FileName;
                siteMapIndexNodes.Add(siteMapIndexNode);

                byte[] bytes = siteMap.SaveAsXml();
                Artifact siteMapAsArtifact = new Artifact(siteMap.FileName, bytes);
                siteMapArtifacts.Add(siteMapAsArtifact);
            }

            SiteMapIndex siteMapIndex = new SiteMapIndex("sitemap_index.xml", siteMapIndexNodes);
            byte[] siteMapIndexBytes = siteMapIndex.SaveAsXml();
            Artifact siteMapIndexAsArtifact = new Artifact(siteMapIndex.FileName, siteMapIndexBytes);

            List<Artifact> artifacts = new List<Artifact>();
            artifacts.AddRange(siteMapArtifacts);
            artifacts.Add(siteMapIndexAsArtifact);

            Artifact[] result = artifacts.ToArray();
            return result;
        }
    }

    public class FeedSiteArtifactPlugin : ISiteArtifactPlugin
    {
        readonly FeedGenerator _FeedGenerator;

        public FeedSiteArtifactPlugin(FeedGenerator feedGenerator)
        {
            _FeedGenerator = feedGenerator;
        }

        public Artifact[] Generate(SiteMetaData siteMetaData)
        {
            System.ServiceModel.Syndication.SyndicationFeed feed = _FeedGenerator.Create(siteMetaData);
            byte[] bytes = feed
                    .SaveAsAtom10();
            Artifact feedAsArtifact = new Artifact("feed.xml", bytes);
            Artifact[] result = [feedAsArtifact];
            return result;
        }
    }
}
