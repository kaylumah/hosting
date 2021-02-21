using System.Collections.Generic;

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
    }
}