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

        public static IEnumerable<T?> GetValues<T>(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            string lookupKey = dictionary.LookupKey(key, caseInsensitive);

            if (!dictionary.TryGetValue(lookupKey, out object? value))
            {
                return [];
            }

            if (value is null)
            {
                return [];
            }

            if (value is IEnumerable<T> exactMatch)
            {
                return exactMatch;
            }

            if (value is IEnumerable enumerable and not string)
            {
                List<T?> result = new List<T?>();
                foreach (object original in enumerable)
                {
                    T? converted = (T?)original.ConvertValue(typeof(T));
                    result.Add(converted);
                }

                return result;
            }

            // Fallback: delegate to GetValue<T?> and wrap in list
            T? singleValue = dictionary.GetValue<T>(key, caseInsensitive);
            List<T?> single = [singleValue];
            return single;
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
