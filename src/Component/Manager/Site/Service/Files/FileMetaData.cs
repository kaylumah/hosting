using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class FileMetaData : Dictionary<string, object>
    {

        private T GetValue<T>(string key) where T : class
        {
            TryGetValue(key.ToLower(), out object o);
            if (o is T t)
            {
                return t;
            }
            return null;
        }

        private void SetValue(string key, object value)
        {
            this[key.ToLower()] = value;
        }

        public string Layout
        {
            get
            {
                return GetValue<string>(nameof(Layout));
            }
            set
            {
                SetValue(nameof(Layout), value);
            }
        }

        public string Permalink
        {
            get
            {
                return GetValue<string>(nameof(Permalink));
            }
            set
            {
                SetValue(nameof(Permalink), value);
            }
        }

        public string Uri
        {
            get
            {
                return GetValue<string>(nameof(Uri));
            }
            set
            {
                SetValue(nameof(Uri), value);
            }
        }
    }
}