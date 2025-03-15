// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("PageMetaData '{Uri}'")]
    public abstract class BasePage
    {
        public static implicit operator Dictionary<string, object?>(BasePage page) => page._InternalData;
        readonly Dictionary<string, object?> _InternalData;

        public BasePage(Dictionary<string, object?> internalData)
        {
            _InternalData = internalData;
        }

        protected string GetString(string key)
        {
            string? result = _InternalData.GetValue<string>(key);
            // TODO fix NULL suppression
            return result!;
        }

        protected int GetInt(string key)
        {
            int result = _InternalData.GetValue<int>(key);
            return result;
        }

        protected TimeSpan GetTimeSpan(string key)
        {
            TimeSpan result = _InternalData.GetValue<TimeSpan>(key);
            return result;
        }

        protected bool GetBoolValue(string key)
        {
            bool result = _InternalData.GetValue<bool>(key);
            return result;
        }

        protected DateTimeOffset GetDateTimeOffsetValue(string key)
        {
            DateTimeOffset result = _InternalData.GetValue<DateTimeOffset>(key);
            return result;
        }

        protected List<string> GetStringValues(string key)
        {
            IEnumerable<string>? values = _InternalData.GetValues<string>(key);
            List<string> result = values?.ToList() ?? new List<string>();
            return result;
        }

        protected void SetValue(string key, object? value)
        {
            _InternalData.SetValue(key, value);
        }

        public string Uri => GetString(nameof(Uri));

        public string BaseUri => GetString(nameof(BaseUri));

        public Uri CanonicalUri => GetCanonicalUri();

        public string Content
        {
            get
            {
                string result = GetString(nameof(Content));
                return result;
            }
            set
            {
                SetValue(nameof(Content), value);
            }
        }

        public string Type
        {
            get
            {
                string result = GetString(nameof(Type));
                return result;
            }
            set
            {
                SetValue(nameof(Type), value);
            }
        }

        Uri GetCanonicalUri()
        {
            string baseUrl = BaseUri;
            Uri result;
            if ("index.html".Equals(Uri, StringComparison.OrdinalIgnoreCase))
            {
                result = new Uri(baseUrl);
            }
            else
            {
                result = RenderHelperFunctions.AbsoluteUri(baseUrl, Uri);
            }

            return result;
        }
    }

    public class StaticContent : BasePage
    {
        public StaticContent(Dictionary<string, object?> internalData) : base(internalData)
        {
        }
    }

    public readonly record struct PageId(string Value)
    {
        public static implicit operator string(PageId pageId) => pageId.Value;
        public static implicit operator PageId(string value) => new(value);
    }

    public class PageMetaData : BasePage
    {
        public PageMetaData(Dictionary<string, object?> internalData) : base(internalData)
        {
        }

        public PageId Id
        {
            get
            {
                string result = GetString(nameof(Id));
                return result;
            }
            set
            {
                string strValue = value;
                SetValue(nameof(Id), strValue);
            }
        }
        public string Title => GetString(nameof(Title));
        public string Description => GetString(nameof(Description));
        public string Language => GetString(nameof(Language));
        public AuthorId Author => GetString(nameof(Author));
        public OrganizationId Organization => GetString(nameof(Organization));
        public bool Sitemap => GetBoolValue(nameof(Sitemap));

        public string LdJson
        {
            get
            {
                string result = GetString(nameof(LdJson));
                return result;
            }
            set
            {
                SetValue(nameof(LdJson), value);
            }
        }
        public string MetaTags
        {
            get
            {
                string result = GetString(nameof(MetaTags));
                return result;
            }
            set
            {
                SetValue(nameof(MetaTags), value);
            }
        }
        public string Layout => GetString(nameof(Layout));
        public string Image => GetString(nameof(Image));
        public Uri? WebImage => ResolveImageUri();

        Uri? ResolveImageUri()
        {
            string? image = Image;
            if (string.IsNullOrEmpty(image))
            {
                return null;
            }

            Uri result = RenderHelperFunctions.AbsoluteUri(BaseUri, image);
            return result;
        }

        public string Name
        {
            get
            {
                string result = GetString(nameof(Name));
                return result;
            }
            set
            {
                SetValue(nameof(Name), value);
            }
        }

        public string Collection
        {
            get
            {
                string result = GetString(nameof(Collection));
                return result;
            }
            set
            {
                SetValue(nameof(Collection), value);
            }
        }

        public List<string> Tags
        {
            get
            {
                List<string>? tags = GetStringValues(nameof(Tags));
                return tags;
            }
            set
            {
                SetValue(nameof(Tags), value);
            }
        }

        public DateTimeOffset Published => GetPublishedDate();
        public DateTimeOffset Modified => GetModifiedDate();

        protected virtual DateTimeOffset GetPublishedDate()
        {
            DateTimeOffset result = GetDateTimeOffsetValue(nameof(Published));
            return result;
        }

        protected virtual DateTimeOffset GetModifiedDate()
        {
            DateTimeOffset result = GetDateTimeOffsetValue(nameof(Modified));
            return result;
        }
    }

    public class CollectionPage : PageMetaData
    {
        readonly IEnumerable<PageMetaData> _Pages;

        public CollectionPage(PageMetaData internalData, IEnumerable<PageMetaData> pages) : base(internalData)
        {
            _Pages = pages.ByRecentlyPublished();
        }

        protected override DateTimeOffset GetPublishedDate()
        {
            // if there is a published date it should win? otherwise fall back on the oldest page
            PageMetaData? firstPublishedPage = _Pages.LastOrDefault();
            DateTimeOffset basePublishedDate = base.GetPublishedDate();
            DateTimeOffset? firstPublishedDate = firstPublishedPage?.Published;
            DateTimeOffset result = firstPublishedDate ?? basePublishedDate;
            return result;
        }

        protected override DateTimeOffset GetModifiedDate()
        {
            // if there is a newer published date it should win? otherwise fall back on newest page
            PageMetaData? lastPublishedPage = _Pages.FirstOrDefault();
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
    }

    public class Article : PageMetaData
    {
        public bool SocialShare => GetBoolValue(nameof(SocialShare));
        public bool Feed => GetBoolValue(nameof(Feed));
        public bool Featured => GetBoolValue(nameof(Featured));
        public string CommentId => GetString(nameof(CommentId));

        public string Series
        {
            get
            {
                string result = GetString(nameof(Series));
                return result;
            }
            set
            {
                SetValue(nameof(Series), value);
            }
        }

        public int NumberOfWords
        {
            get
            {
                int result = GetInt(nameof(NumberOfWords));
                return result;
            }
            set
            {
                SetValue(nameof(NumberOfWords), value);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan result = GetTimeSpan(nameof(Duration));
                return result;
            }
            set
            {
                SetValue(nameof(Duration), value);
            }
        }

        public Article(Dictionary<string, object?> internalData) : base(internalData)
        {
        }
    }
}
