﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata
{
    public class FileMetaData : Dictionary<string, object?>
    {
        public string? Layout
        {
            get
            {
                string? layout = this.GetValue<string>(nameof(Layout));
                return layout;
            }
            set
            {
                this.SetValue(nameof(Layout), value);
            }
        }

        public string? OutputLocation
        {
            get
            {
                string? outputLocation = this.GetValue<string>(nameof(OutputLocation));
                return outputLocation;
            }
            set
            {
                this.SetValue(nameof(OutputLocation), value);
            }
        }

        public string Uri
        {
            get
            {
                string? uri = this.GetValue<string>(nameof(Uri));
                if (uri == null)
                {
                    throw new InvalidOperationException("URI is required");
                }

                return uri;
            }
            set
            {
                this.SetValue(nameof(Uri), value);
            }
        }

        public string? Collection
        {
            get
            {
                string? collection = this.GetValue<string>(nameof(Collection));
                return collection;
            }
            set
            {
                this.SetValue(nameof(Collection), value);
            }
        }

        public DateTimeOffset? Date
        {
            get
            {
                DateTimeOffset? date = this.GetValue<DateTimeOffset?>(nameof(Date));
                return date;
            }
            set
            {
                this.SetValue(nameof(Date), value);
            }
        }

        public string? PublishedDate
        {
            get
            {
                string? publishedDate = this.GetValue<string>(nameof(PublishedDate));
                return publishedDate;
            }
            set
            {
                this.SetValue(nameof(PublishedDate), value);
            }
        }

        public string? PublishedTime
        {
            get
            {
                string? publishTime = this.GetValue<string>(nameof(PublishedTime));
                return publishTime;
            }
            set
            {
                this.SetValue(nameof(PublishedTime), value);
            }
        }

        public DateTimeOffset? Published
        {
            get
            {
                DateTimeOffset? published = this.GetValue<DateTimeOffset?>(nameof(Published));
                return published;
            }
            set
            {
                this.SetValue(nameof(Published), value);
            }
        }

        public string? ModifiedDate
        {
            get
            {
                string? modifiedDate = this.GetValue<string>(nameof(ModifiedDate));
                return modifiedDate;
            }
            set
            {
                this.SetValue(nameof(ModifiedDate), value);
            }
        }

        public string? ModifiedTime
        {
            get
            {
                string? modifiedTime = this.GetValue<string>(nameof(ModifiedTime));
                return modifiedTime;
            }
            set
            {
                this.SetValue(nameof(ModifiedTime), value);
            }
        }

        public DateTimeOffset? Modified
        {
            get
            {
                DateTimeOffset? modified = this.GetValue<DateTimeOffset?>(nameof(Modified));
                return modified;
            }
            set
            {
                this.SetValue(nameof(Modified), value);
            }
        }
    }
}
