// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata
{
    public class FileMetaData : Dictionary<string, object?>
    {

        public string Series
        {
            get
            {
                string series = this.GetValue<string>(nameof(Series));
                return series;
            }
            set
            {
                this.SetValue(nameof(Series), value);
            }
        }

        public string Layout
        {
            get
            {
                string layout = this.GetValue<string>(nameof(Layout));
                return layout;
            }
            set
            {
                this.SetValue(nameof(Layout), value);
            }
        }

        public string OutputLocation
        {
            get
            {
                string outputLocation = this.GetValue<string>(nameof(OutputLocation));
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
                string uri = this.GetValue<string>(nameof(Uri));
                return uri;
            }
            set
            {
                this.SetValue(nameof(Uri), value);
            }
        }

        public string SourceFileName
        {
            get
            {
                string sourceFileName = this.GetValue<string>(nameof(SourceFileName));
                return sourceFileName;
            }
            set
            {
                this.SetValue(nameof(SourceFileName), value);
            }
        }

        public string Raw
        {
            get
            {
                string raw = this.GetValue<string>(nameof(Raw));
                return raw;
            }
            set
            {
                this.SetValue(nameof(Raw), value);
            }
        }

        public string Content
        {
            get
            {
                string content = this.GetValue<string>(nameof(Content));
                return content;
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
                string collection = this.GetValue<string>(nameof(Collection));
                return collection;
            }
            set
            {
                this.SetValue(nameof(Collection), value);
            }
        }

        public List<string> Tags
        {
            get
            {
                List<object?> result = this.GetValue<List<object?>>(nameof(Tags));
                IEnumerable<string?> strings = result.Cast<string?>();
                List<string> asList = strings
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToList();
                return asList;
            }
            set
            {
                this.SetValue(nameof(Tags), value);
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

        public string PublishedDate
        {
            get
            {
                string publishedDate = this.GetValue<string>(nameof(PublishedDate));
                return publishedDate;
            }
            set
            {
                this.SetValue(nameof(PublishedDate), value);
            }
        }

        public string PublishedTime
        {
            get
            {
                string publishTime = this.GetValue<string>(nameof(PublishedTime));
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

        public string ModifiedDate
        {
            get
            {
                string modified = this.GetValue<string>(nameof(ModifiedDate));
                return modified;
            }
            set
            {
                this.SetValue(nameof(ModifiedDate), value);
            }
        }

        public string ModifiedTime
        {
            get
            {
                string time = this.GetValue<string>(nameof(ModifiedTime));
                return time;
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
