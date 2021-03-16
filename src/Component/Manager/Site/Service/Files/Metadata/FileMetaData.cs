// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class FileMetaData : Dictionary<string, object>
    {

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

        public string Permalink
        {
            get
            {
                return this.GetValue<string>(nameof(Permalink));
            }
            set
            {
                this.SetValue(nameof(Permalink), value);
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
    }
}