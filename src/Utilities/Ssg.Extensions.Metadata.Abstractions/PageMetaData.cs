// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("PageMetaData '{Uri}'")]
    public class PageMetaData : Dictionary<string, object>
    {
        public string Id
        {
            get
            {
                string result = this.GetValue<string>(nameof(Id));
                return result;
            }
            set
            {
                this.SetValue(nameof(Id), value);
            }
        }
        public string Title => this.GetValue<string>(nameof(Title));
        public string Description => this.GetValue<string>(nameof(Description));
        public string Language => this.GetValue<string>(nameof(Language));
        public string Author => this.GetValue<string>(nameof(Author));
        public string Organization => this.GetValue<string>(nameof(Organization));
        public bool SocialShare => this.GetBoolValue(nameof(SocialShare));
        public bool Sitemap => this.GetBoolValue(nameof(Sitemap));
        public bool Feed => this.GetBoolValue(nameof(Feed));
        public bool Featured => this.GetBoolValue(nameof(Featured));
        public string LdJson
        {
            get
            {
                string result = this.GetValue<string>(nameof(LdJson));
                return result;
            }
            set
            {
                this.SetValue(nameof(LdJson), value);
            }
        }
        public string MetaTags
        {
            get
            {
                string result = this.GetValue<string>(nameof(MetaTags));
                return result;
            }
            set
            {
                this.SetValue(nameof(MetaTags), value);
            }
        }
        public string Layout => this.GetValue<string>(nameof(Layout));
        public string Uri => this.GetValue<string>(nameof(Uri));
        public string Image => this.GetValue<string>(nameof(Image));
        public string CommentId => this.GetValue<string>(nameof(CommentId));

        public string Name
        {
            get
            {
                string result = this.GetValue<string>(nameof(Name));
                return result;
            }
            set
            {
                this.SetValue(nameof(Name), value);
            }
        }

        public string Content
        {
            get
            {
                string result = this.GetValue<string>(nameof(Content));
                return result;
            }
            set
            {
                this.SetValue(nameof(Content), value);
            }
        }

        public string Collection
        {
            get
            {
                string result = this.GetValue<string>(nameof(Collection));
                return result;
            }
            set
            {
                this.SetValue(nameof(Collection), value);
            }
        }

        public string Series
        {
            get
            {
                string result = this.GetValue<string>(nameof(Series));
                return result;
            }
            set
            {
                this.SetValue(nameof(Series), value);
            }
        }

        public List<string> Tags
        {
            get
            {
                List<string> tags = this.GetValue<List<object>>(nameof(Tags))?.Cast<string>().ToList();
                return tags ?? new List<string>();
            }
            set
            {
                this.SetValue(nameof(Tags), value);
            }
        }

        public ContentType Type
        {
            get
            {
                string contentType = this.GetValue<string>(nameof(Type));
                ContentType x = Enum.Parse<ContentType>(contentType);
                return x;
            }
            set
            {
                this.SetValue(nameof(Type), value);
            }
        }

        public DateTimeOffset Published => this.GetValue<DateTimeOffset>(nameof(Published));
        public DateTimeOffset Modified => this.GetValue<DateTimeOffset>(nameof(Modified));

        public PageMetaData(Dictionary<string, object> internalData) : base(internalData)
        {
        }
    }
}
