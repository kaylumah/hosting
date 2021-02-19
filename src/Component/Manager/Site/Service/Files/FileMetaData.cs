using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class FileMetaData : Dictionary<string, object>
    {
        public string Layout
        {
            get
            {
                if (ContainsKey(nameof(Layout).ToLower()))
                {
                    return (string)this[nameof(Layout).ToLower()];
                }
                return null;
            }
            set
            {
                this[nameof(Layout).ToLower()] = value;
            }
        }

        public string Permalink
        {
            get
            {
                if (ContainsKey(nameof(Permalink).ToLower()))
                {
                    return (string)this[nameof(Permalink).ToLower()];
                }
                return null;
            }
            set
            {
                this[nameof(Permalink).ToLower()] = value;
            }
        }

        public string Uri
        {
            get
            {
                if (ContainsKey(nameof(Uri).ToLower()))
                {
                    return (string)this[nameof(Uri).ToLower()];
                }
                return null;
            }
            set
            {
                this[nameof(Uri).ToLower()] = value;
            }
        }
    }
}