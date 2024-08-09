// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service.Search
{
    public class IndexItem
    {
        public string Id
        { get; set; }

        public string Title
        { get; set; }

        public IndexItem(string id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}