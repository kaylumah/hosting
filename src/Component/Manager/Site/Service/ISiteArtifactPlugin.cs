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
        var sitemap = _siteMapGenerator.Create(siteMetaData);
        var bytes = sitemap
                .SaveAsXml();
        var siteMapAsArtifact = new Artifact
        {
            Contents = bytes,
            Path = "sitemap.xml"
        };
        var result = new Artifact[] {
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
        var feed = _feedGenerator.Create(siteMetaData);
        var bytes = feed
                .SaveAsAtom10();
        var feedAsArtifact = new Artifact
        {
            Contents = bytes,
            Path = "feed.xml"
        };
        var result = new Artifact[] {
            feedAsArtifact
        };
        return result;
    }
}
