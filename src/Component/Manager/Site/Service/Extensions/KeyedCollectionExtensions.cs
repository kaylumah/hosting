// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

// ReSharper disable once CheckNamespace
namespace System.Collections.ObjectModel;

public static class KeyedCollectionExtensions
{
    public static void AddRange<TKey,TItem>(this KeyedCollection<TKey, TItem> source, IEnumerable<TItem> items)
    {
        _ = items ?? throw new ArgumentNullException(nameof(items));

        foreach (var obj in items)
        {
            source.Add(obj);
        }
    }
}
