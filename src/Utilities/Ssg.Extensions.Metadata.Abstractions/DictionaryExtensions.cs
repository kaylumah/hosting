// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Linq;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static T? GetValue<T>(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            string lookupKey = caseInsensitive ? key.ToLower(CultureInfo.InvariantCulture) : key;
            if (!dictionary.TryGetValue(lookupKey, out object? value))
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

            try
            {
                return (T)ConvertValue(value, typeof(T));
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidOperationException($"Cannot convert value of key '{key}' from {value?.GetType()} to {typeof(T)}.", ex);
            }
        }

        public static IEnumerable<T>? GetValues<T>(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            string lookupKey = caseInsensitive ? key.ToLower(CultureInfo.InvariantCulture) : key;
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
                return new List<T> { singleValue };
            }

            if (value is IEnumerable<object> objectList)
            {
                try
                {
                    return objectList.Select(item => (T)ConvertValue(item, typeof(T)));
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidOperationException($"Cannot convert list elements of key '{key}' to {typeof(T)}.", ex);
                }
            }

            throw new InvalidOperationException($"Cannot convert value of key '{key}' from {value?.GetType()} to IEnumerable<{typeof(T)}>.");
        }

        static object ConvertValue(object value, Type targetType)
        {
            if (value is string strValue)
            {
                if (targetType == typeof(bool) && bool.TryParse(strValue, out bool boolResult))
                {
                    return boolResult;
                }

                if (targetType == typeof(int) && int.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out int intResult))
                {
                    return intResult;
                }

                if (targetType == typeof(Guid) && Guid.TryParse(strValue, out Guid guidResult))
                {
                    return guidResult;
                }

                /*
                if (targetType == typeof(DateTime) && DateTime.TryParse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dateTimeResult))
                {
                    return dateTimeResult;
                }
                */

                if (targetType == typeof(TimeSpan) && TimeSpan.TryParse(strValue, CultureInfo.InvariantCulture, out TimeSpan timeSpanResult))
                {
                    return timeSpanResult;
                }
            }

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        
        public static bool GetBoolValue(this Dictionary<string, object?> dictionary, string key)
        {
            string? stringValue = dictionary.GetValue<string>(key);
            bool tryParseOutcome = bool.TryParse(stringValue, out bool parseResult);
            // if the value is not a boolean, we default to false
            return tryParseOutcome && parseResult;
        }

        public static List<string> GetStringValues(this Dictionary<string, object?> dictionary, string key)
        {
            List<object>? internalList = dictionary.GetValue<List<object>>(key);
            List<string>? asStrings = internalList?.Cast<string>()?.ToList();
            List<string> result = asStrings ?? [];
            return result;
        }

        /*
        public static T GetValue<T>(this Dictionary<string, object?> dictionary, string key)
        {
            string lowerKey = key; //.ToLower(CultureInfo.InvariantCulture);
            bool exists = dictionary.TryGetValue(lowerKey, out object? o);

            if (o is T t)
            {
                return t;
            }

            T result = default!;
            return result;
        }
        */

        public static void SetValue(this Dictionary<string, object?> dictionary, string key, object? value)
        {
            string lowerKey = key.ToLower(CultureInfo.InvariantCulture);
            dictionary[lowerKey] = value;
        }
    }
}
