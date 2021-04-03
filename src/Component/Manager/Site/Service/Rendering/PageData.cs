// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Kaylumah.Ssg.Manager.Site.Service.Rendering
{
    public class PageData : Dictionary<string, object>, IPageMetadata
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
        public string Url => this.GetValue<string>(nameof(Url));
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

        public PageData(Dictionary<string, object> metadata, string content, DateTimeOffset lastModified): base(metadata)
        {
            Content = content;
            LastModified = lastModified;
            // TODO sync this...
            this.SetValue("url", this.GetValue<string>("uri"));
        }
    }
}