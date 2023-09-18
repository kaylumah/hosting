// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;

namespace System.Collections.Generic;

public static class DictionaryExtensions
{
    public static bool GetBoolValue(this Dictionary<string, object> dictionary, string key)
    {
        var stringValue = dictionary.GetValue<string>(key);
        var result = bool.Parse(stringValue);
        return result;
    }

    public static T GetValue<T>(this Dictionary<string, object> dictionary, string key)
    {
        var lowerKey = key.ToLower(CultureInfo.InvariantCulture);
        dictionary.TryGetValue(lowerKey, out object o);
        if (o is T t)
        {
            return t;
        }
        var result = default(T);
        return result;
    }

    public static void SetValue(this Dictionary<string, object> dictionary, string key, object value)
    {
        dictionary[key.ToLower(CultureInfo.InvariantCulture)] = value;
    }
}
