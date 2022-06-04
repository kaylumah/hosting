// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

[DebuggerDisplay("PageMetaData '{Uri}'")]
public class PageMetaData : Dictionary<string, object>
{
    public string Id
    {
        get
        {
            return this.GetValue<string>(nameof(Id));
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
    public bool Sitemap => this.GetBoolValue(nameof(Sitemap));
    public bool Feed => this.GetBoolValue(nameof(Feed));
    public string PublishedDate => this.GetValue<string>(nameof(PublishedDate));
    public string ModifiedDate => this.GetValue<string>(nameof(ModifiedDate));
    public string LdJson
    {
        get
        {
            return this.GetValue<string>(nameof(LdJson));
        }
        set
        {
            this.SetValue(nameof(LdJson), value);
        }
    }
    public string Layout => this.GetValue<string>(nameof(Layout));
    public string Uri => this.GetValue<string>(nameof(Uri));
    public string Image => this.GetValue<string>(nameof(Image));
    public DateTimeOffset Date => this.GetValue<DateTimeOffset>(nameof(Date));

    public string Name
    {
        get
        {
            return this.GetValue<string>(nameof(Name));
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
            return this.GetValue<string>(nameof(Content));
        }
        set
        {
            this.SetValue(nameof(Content), value);
        }
    }

    public DateTimeOffset LastModified
    {
        get
        {
            return this.GetValue<DateTimeOffset>(nameof(LastModified));
        }
        set
        {
            this.SetValue(nameof(LastModified), value);
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

    public List<string> Tags
    {
        get
        {
            var tags = this.GetValue<List<object>>(nameof(Tags))?.Cast<string>().ToList();
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
            var contentType = this.GetValue<string>(nameof(Type));
            var x = Enum.Parse<ContentType>(contentType);
            return x;
        }
        set
        {
            this.SetValue(nameof(Type), value);
        }
    }

    public PageMetaData(Dictionary<string, object> metadata, string name, string content, DateTimeOffset lastModified) : base(metadata)
    {
        Name = name;
        Content = content;
        LastModified = lastModified;
    }
}
