// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static T? GetValue<T>(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            string lookupKey = dictionary.LookupKey(key, caseInsensitive);

            bool exists = dictionary.TryGetValue(lookupKey, out object? value);
            if (!exists)
            {
                return default;
            }

            if (value is null)
            {
                return default;
            }

            if (value is T exactMatch)
            {
                return exactMatch;
            }

            T? result = (T?)value.ConvertValue(typeof(T));
            return result;
        }

        public static IEnumerable<T?>? GetValues<T>(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            string lookupKey = dictionary.LookupKey(key, caseInsensitive);

            if (!dictionary.TryGetValue(lookupKey, out object? value))
            {
                return default;
            }

            if (value is null)
            {
                return default;
            }

            if (value is IEnumerable<T> exactMatch)
            {
                return exactMatch;
            }

            if (value is T singleValue)
            {
                List<T> result = new List<T>() { singleValue };
                return result;
            }

            if (value is IEnumerable<object> objectList)
            {
                List<T> result = new List<T>();
                foreach (object original in objectList)
                {
                    T? converted = (T?)original.ConvertValue(typeof(T));
                    if (converted != null)
                    {
                        result.Add(converted);
                    }
                }

                return result;
            }

            throw new InvalidOperationException($"Cannot convert value of key '{key}' from {value?.GetType()} to IEnumerable<{typeof(T)}>.");
        }

        public static void SetValue(this Dictionary<string, object?> dictionary, string key, object? value, bool caseInsensitive = true)
        {
            string lookupKey = dictionary.LookupKey(key, caseInsensitive);
            dictionary[lookupKey] = value;
        }

        static string LookupKey(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);
            Debug.Assert(caseInsensitive);
            string lookupKey = caseInsensitive
                ? dictionary.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase)) ?? key
                : key;
            return lookupKey;
        }
    }
}
