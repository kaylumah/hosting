// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class CollectionPageMetaData : PageMetaData
    {
        readonly Dictionary<PageId, PublicationPageMetaData> _Lookup;

        public int Take => GetInt(nameof(Take));

        public IEnumerable<PublicationPageMetaData> Items
        { get; }

        public IEnumerable<ArticlePublicationPageMetaData> RecentArticles => GetRecentArticles();

        public SortedDictionary<int, List<PageId>> PagesByYears => GetPagesByYear();

        public CollectionPageMetaData(PageMetaData internalData, List<PublicationPageMetaData> items) : base(internalData)
        {
            Items = items.ByRecentlyPublished();

            _Lookup = Items
                .ToDictionary(key => key.Id,
                    value => value);
        }

        #region Indexers

        public PublicationPageMetaData? this[PageId pageId]
        {
            get => _Lookup.GetValueOrDefault(pageId);
        }

        public IEnumerable<PublicationPageMetaData> this[IEnumerable<PageId> ids]
        {
            get
            {
                foreach (PageId id in ids)
                {
                    if (_Lookup.TryGetValue(id, out PublicationPageMetaData? page))
                    {
                        yield return page;
                    }
                }
            }
        }

        #endregion

        protected override DateTimeOffset GetPublishedDate()
        {
            IEnumerable<PublicationPageMetaData> pages = Items;
            // if there is a published date it should win? otherwise fall back on the oldest page
            PublicationPageMetaData? firstPublishedPage = pages.LastOrDefault();
            DateTimeOffset basePublishedDate = base.GetPublishedDate();
            DateTimeOffset? firstPublishedDate = firstPublishedPage?.Published;
            DateTimeOffset result = firstPublishedDate ?? basePublishedDate;
            return result;
        }

        protected override DateTimeOffset GetModifiedDate()
        {
            IEnumerable<PublicationPageMetaData> pages = Items;
            // if there is a newer published date it should win? otherwise fall back on newest page
            PublicationPageMetaData? lastPublishedPage = pages.FirstOrDefault();
            // DateTimeOffset defaultDate = DateTimeOffset.MinValue;
            DateTimeOffset baseModifiedDate = base.GetModifiedDate();
            DateTimeOffset? lastPublishedDate = lastPublishedPage?.Published;

            DateTimeOffset result;
            /*
            if (lastPublishedDate == null || lastPublishedDate == defaultDate)
            {
                result = baseModifiedDate;
            }

            if (baseModifiedDate == defaultDate)
            {
                // return lastPublishedDate
            }
            */

            if (baseModifiedDate < lastPublishedDate)
            {
                result = lastPublishedDate.Value;
            }
            else
            {
                result = baseModifiedDate;
            }

            return result;
        }

        #region PageTypes
        IEnumerable<ArticlePublicationPageMetaData> GetArticles()
        {
            IEnumerable<ArticlePublicationPageMetaData> articles = Items.OfType<ArticlePublicationPageMetaData>();

            if (0 < Take)
            {
                articles = articles.Take(Take);
            }

            return articles;
        }

        #endregion

        #region Collections

        IEnumerable<ArticlePublicationPageMetaData> GetRecentArticles()
        {
            IEnumerable<ArticlePublicationPageMetaData> articles = GetArticles();
            IEnumerable<ArticlePublicationPageMetaData> sortedByPublished = articles.ByRecentlyPublished();
            return sortedByPublished;
        }

        #endregion

        #region CrossSections

        SortedDictionary<int, List<PageId>> GetPagesByYear()
        {
            IEnumerable<PublicationPageMetaData> items = Items;
            SortedDictionary<int, List<PageId>> result = items.GetPagesByYear();
            return result;
        }

        #endregion
    }
}