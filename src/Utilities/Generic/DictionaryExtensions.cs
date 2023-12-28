
// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static bool GetBoolValue(this Dictionary<string, object> dictionary, string key)
        {
            string stringValue = dictionary.GetValue<string>(key);
            bool result = bool.Parse(stringValue);
            return result;
        }

        public static T GetValue<T>(this Dictionary<string, object> dictionary, string key)
        {
            string lowerKey = key.ToLower(CultureInfo.InvariantCulture);
            dictionary.TryGetValue(lowerKey, out object o);
            if (o is T t)
            {
                return t;
            }

            T result = default(T);
            return result;
        }

        public static void SetValue(this Dictionary<string, object> dictionary, string key, object value)
        {
            string lowerKey = key.ToLower(CultureInfo.InvariantCulture);
            dictionary[lowerKey] = value;
        }
    }
}
