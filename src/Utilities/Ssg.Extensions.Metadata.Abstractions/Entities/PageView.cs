// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class PageView
    {
        public string Id
        { get; set; }

        public string DisplayName
        { get; set; }

        public PageMetaData[] Pages
        { get; set; }

        public int Size => Pages.Length;

        public PageView(string id, string displayName, PageMetaData[] pages)
        {
            Id = id;
            DisplayName = displayName;
            Pages = pages;
        }
    }

    public class PageViewCollection : KeyedCollection<string, PageView>
    {
        protected override string GetKeyForItem(PageView item)
        {
            return item.Id;
        }

        public new IDictionary<string, PageView> Dictionary => base.Dictionary!;

        public IEnumerable<string> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<string>();

    }
}