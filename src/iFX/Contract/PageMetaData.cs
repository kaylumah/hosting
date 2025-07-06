// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public class PageMetaData : BasePage
    {
        public PageMetaData(Dictionary<string, object?> internalData) : base(internalData)
        {
        }

        public PageId Id => GetString(nameof(Id));
        public string Title => GetString(nameof(Title));
        public string Description => GetString(nameof(Description));
        public string Language => GetString(nameof(Language));
        public AuthorId Author => GetString(nameof(Author));
        public OrganizationId Organization => GetString(nameof(Organization));
        public bool Sitemap => GetBoolValue(nameof(Sitemap));
        public bool Ads => GetBoolValue(nameof(Ads));

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

        public string Name => GetString(nameof(Name));

        public string Collection => GetString(nameof(Collection));

        public List<string> Keywords => GetStringValues(nameof(Keywords));

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
}
