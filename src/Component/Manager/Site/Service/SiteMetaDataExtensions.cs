// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;

namespace System.ServiceModel.Syndication
{
    public static partial class SiteMetaDataExtensions
    {
        public static SyndicationFeed ToSyndicationFeed(this SiteMetaData siteMetaData)
        {
            // See:
            // https://docs.microsoft.com/en-us/dotnet/api/system.servicemodel.syndication.syndicationfeed?view=dotnet-plat-ext-6.0
            // https://github.com/kestrelblackmore/BlogMatrix/blob/master/App_Code/RssSyndicator.cs
            // https://github.com/kestrelblackmore/BlogMatrix/blob/master/feed.cshtml
            // https://validator.w3.org/feed/check.cgi
            // https://dzone.com/articles/systemservicemodelsyndication
            // https://khalidabuhakmeh.com/reading-rss-feeds-with-dotnet-core

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

            var persons = new Dictionary<string, SyndicationPerson>();
            if (siteMetaData.Data.TryGetValue("authors", out var authorData))
            {
                if (authorData is Dictionary<object, object> authors)
                {
                    foreach(var author in authors)
                    {
                        var singleDictionary = (Dictionary<object, object>)author.Value;
                        var syndicationPerson = new SyndicationPerson();
                        syndicationPerson.Name = (string)singleDictionary["full_name"];
                        syndicationPerson.Email = (string)singleDictionary["email"];
                        syndicationPerson.Uri = (string)singleDictionary["uri"];
                        persons.Add((string)author.Key, syndicationPerson);
                    }
                }
            }

            /*
            SyndicationLink link = new SyndicationLink(new Uri("http://server/link"), "alternate", "Link Title", "text/html", 1000);
            feed.Links.Add(link);
            */

            /*
            var feed = new SyndicationFeed();
            feed.Language = siteMetaData.Language;
            feed.Id = siteMetaData.Id;
            feed.Title = new TextSyndicationContent(siteMetaData.Title);
            */


            if (siteMetaData.Collections.TryGetValue("posts", out var posts))
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
                foreach( var pageMetaData in posts)
                {
                    var author = persons[pageMetaData.Author];
                    var pageUrl = GlobalFunctions.AbsoluteUrl(pageMetaData.Url);
                    var item = new SyndicationItem
                    { 
                        Id = pageUrl,
                        Title = new TextSyndicationContent(pageMetaData.Title),
                        Summary = new TextSyndicationContent(pageMetaData.Description),
                        // Content = new TextSyndicationContent(pageMetaData.Content, TextSyndicationContentKind.Html),
                        PublishDate = (DateTimeOffset)pageMetaData["date"],
                        LastUpdatedTime = pageMetaData.LastModified
                    };

                    foreach (var tag in pageMetaData.Tags)
                    {
                        item.Categories.Add(new SyndicationCategory(tag));
                    }
                    
                    
                    item.Authors.Add(author);
                    items.Add(item);
                }
                feed.Items = items;
            }

            feed.Items = feed.Items.OrderByDescending(x => x.PublishDate);
            return feed;
        }
    }
}
