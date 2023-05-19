// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;

public class FileMetaData : Dictionary<string, object>
{

    public string Series
    {
        get
        {
            return this.GetValue<string>(nameof(Series));
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
            return this.GetValue<string>(nameof(Layout));
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
            return this.GetValue<string>(nameof(OutputLocation));
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
            return this.GetValue<string>(nameof(Uri));
        }
        set
        {
            this.SetValue(nameof(Uri), value);
        }
    }

    public string Collection
    {
        get
        {
            return this.GetValue<string>(nameof(Collection));
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
            return this.GetValue<List<object>>(nameof(Tags))?.Cast<string>().ToList();
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
            return this.GetValue<DateTimeOffset?>(nameof(Date));
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
            return this.GetValue<string>(nameof(PublishedDate));
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
            return this.GetValue<string>(nameof(PublishedTime));
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
            return this.GetValue<DateTimeOffset?>(nameof(Published));
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
            return this.GetValue<string>(nameof(ModifiedDate));
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
            return this.GetValue<string>(nameof(ModifiedTime));
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
            return this.GetValue<DateTimeOffset?>(nameof(Modified));
        }
        set
        {
            this.SetValue(nameof(Modified), value);
        }
    }
}
