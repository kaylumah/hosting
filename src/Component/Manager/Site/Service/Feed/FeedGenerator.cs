// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ServiceModel.Syndication;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Manager.Site.Service.Feed
{
    public class FeedGenerator
    {
        private readonly ILogger _logger;

        public FeedGenerator(ILogger<FeedGenerator> logger)
        {
            _logger = logger;
        }

        private IEnumerable<PageMetaData> RetrievePostPageMetaDatas(SiteMetaData siteMetaData)
        {
            if (siteMetaData.Collections.TryGetValue("posts", out var posts))
            {
                return posts;
            }
            return Enumerable.Empty<PageMetaData>();
        }

        public SyndicationFeed Create(SiteMetaData siteMetaData)
        {
            var build = siteMetaData.Build;
            var generatorVersion = build.ShortGitHash;
            var copyrightClaim = build.Copyright;
            var generatedAtBuildTime = build.Time;

            var feed = new SyndicationFeed();
            feed.Language = siteMetaData.Language;
            feed.Title = new TextSyndicationContent(siteMetaData.Title);
            feed.Description = new TextSyndicationContent(siteMetaData.Description);
            feed.Id = GlobalFunctions.AbsoluteUrl("feed.xml");
            feed.Copyright = new TextSyndicationContent(copyrightClaim);
            feed.LastUpdatedTime = generatedAtBuildTime;
            feed.ImageUrl = new Uri(GlobalFunctions.AbsoluteUrl("assets/logo_alt.svg"));
            feed.Generator = "Kaylumah Site Generator";

            feed.Links.Add(new SyndicationLink(new Uri(GlobalFunctions.AbsoluteUrl("feed.xml")))
            {
                RelationshipType = "self",
                MediaType = "application/atom+xml",
            });

            /*
            feed.Links.Add(new SyndicationLink(new Uri(GlobalFunctions.Instance.Url))
            {
                MediaType = "text/html",
            });
            */

            feed.Links.Add(new SyndicationLink(new Uri(GlobalFunctions.AbsoluteUrl("blog.html")))
            {
                RelationshipType = "alternate",
                MediaType = "text/html",
            });

            feed.Links.Add(new SyndicationLink(new Uri(GlobalFunctions.AbsoluteUrl("archive.html")))
            {
                RelationshipType = "related",
                MediaType = "text/html",
            });

            var persons = new Dictionary<string, SyndicationPerson>();
            if (siteMetaData.Data.TryGetValue("authors", out var authorData))
            {
                if (authorData is Dictionary<object, object> authors)
                {
                    foreach (var author in authors)
                    {
                        var singleDictionary = (Dictionary<object, object>)author.Value;
                        var syndicationPerson = new SyndicationPerson
                        {
                            Name = (string)singleDictionary["full_name"],
                            Email = (string)singleDictionary["email"],
                            Uri = (string)singleDictionary["uri"]
                        };
                        persons.Add((string)author.Key, syndicationPerson);
                    }
                }
            }

            var posts = RetrievePostPageMetaDatas(siteMetaData);
            if (posts.Any())
            {
                var tags = siteMetaData.TagMetaData
                    .ToDictionary(x => x.Id, x => new SyndicationCategory(x.Name));
                /*
                var feedPosts = posts
                    .OrderByDescending(x => x["date"])
                    .Where(x => bool.Parse((string)x["feed"]))
                    .ToList();
                feed.Items = posts.ToSyndicationItems();
                */

                var items = new List<SyndicationItem>();
                foreach (var pageMetaData in posts)
                {
                    //if (!string.Equals("2020/08/01/kaylumah-the-new-home-for-blogs-written-by-max-hamulyak.html", pageMetaData.Url))
                    //{
                        var author = persons[pageMetaData.Author];
                        var pageUrl = GlobalFunctions.AbsoluteUrl(pageMetaData.Url);
                        var item = new SyndicationItem
                        {
                            Id = pageUrl,
                            Title = new TextSyndicationContent(pageMetaData.Title),
                            Summary = new TextSyndicationContent(pageMetaData.Description),
                            Content = new CDataSyndicationContent(new TextSyndicationContent(pageMetaData.Content, TextSyndicationContentKind.Html)),
                            PublishDate = (DateTimeOffset)pageMetaData["date"],
                            LastUpdatedTime = pageMetaData.LastModified
                        };

                        var itemCategories = pageMetaData
                            .Tags
                            .Where(tag => tags.ContainsKey(tag))
                            .Select(tag => tags[tag])
                            .ToList();
                        itemCategories.ForEach(category => item.Categories.Add(category));
                        item.Links.Add(new SyndicationLink(new Uri(pageUrl)));
                        item.Authors.Add(author);
                        items.Add(item);
                    //}
                }
                feed.Items = items;
            }

            feed.Items = feed.Items.OrderByDescending(x => x.PublishDate);
            return feed;
        }
    }
}
