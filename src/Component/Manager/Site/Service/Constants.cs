// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service
{
    static class Constants
    {
        internal static class KnownFiles
        {
            internal const string Authors = "authors.yml";
            internal const string Organizations = "organizations.yml";
            internal const string Tags = "tags.yml";
        }

        internal static class Directories
        {
            internal const string SourceDirectory = "_site";
            internal const string DestinationDirectory = "dist";
            internal const string AssetDirectory = "assets";
            internal const string DataDirectory = "_data";
            internal const string LayoutDirectory = "_layouts";
            internal const string PartialsDirectory = "_partials";
            internal const string PostDirectory = "_posts";
            internal const string PageDirectory = "_pages";
        }
    }
}
