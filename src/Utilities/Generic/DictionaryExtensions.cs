// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;

namespace System.Collections.Generic;

public static class DictionaryExtensions
{
    public static T GetValue<T>(this Dictionary<string, object> dictionary, string key)
    {
        dictionary.TryGetValue(key.ToLower(CultureInfo.InvariantCulture), out object o);
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
