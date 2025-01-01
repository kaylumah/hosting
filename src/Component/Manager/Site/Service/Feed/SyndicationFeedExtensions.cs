﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Kaylumah.Ssg.Manager.Site.Service.Feed
{
    public static class SyndicationFeedExtensions
    {
        public static byte[] SaveAsAtom10(this FeedArtifact feed)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new System.Text.UTF8Encoding(false);
            using MemoryStream stream = new MemoryStream();
            using XmlWriter writer = XmlWriter.Create(stream, settings);
            SyndicationFeed syndicationFeed = feed.SyndicationFeed;
            syndicationFeed.SaveAsAtom10(writer);
            writer.Close();
            byte[] result = stream.ToArray();
            return result;
        }
    }
}
