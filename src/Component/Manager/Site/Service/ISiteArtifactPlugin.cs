// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service;

public interface ISiteArtifactPlugin
{
    Artifact[] Generate(SiteMetaData siteMetaData);
}

public class SiteMapSiteArtifactPlugin : ISiteArtifactPlugin
{
    private readonly SiteMapGenerator _siteMapGenerator;

    public SiteMapSiteArtifactPlugin(SiteMapGenerator siteMapGenerator)
    {
        _siteMapGenerator = siteMapGenerator;
    }

    public Artifact[] Generate(SiteMetaData siteMetaData)
    {
        SiteMap.SiteMap sitemap = _siteMapGenerator.Create(siteMetaData);
        byte[] bytes = sitemap
                .SaveAsXml();
        Artifact siteMapAsArtifact = new Artifact
        {
            Contents = bytes,
            Path = "sitemap.xml"
        };
        Artifact[] result = new Artifact[] {
            siteMapAsArtifact
        };
        return result;
    }
}

public class FeedSiteArtifactPlugin : ISiteArtifactPlugin
{
    private readonly FeedGenerator _feedGenerator;

    public FeedSiteArtifactPlugin(FeedGenerator feedGenerator)
    {
        _feedGenerator = feedGenerator;
    }

    public Artifact[] Generate(SiteMetaData siteMetaData)
    {
        System.ServiceModel.Syndication.SyndicationFeed feed = _feedGenerator.Create(siteMetaData);
        byte[] bytes = feed
                .SaveAsAtom10();
        Artifact feedAsArtifact = new Artifact
        {
            Contents = bytes,
            Path = "feed.xml"
        };
        Artifact[] result = new Artifact[] {
            feedAsArtifact
        };
        return result;
    }
}
