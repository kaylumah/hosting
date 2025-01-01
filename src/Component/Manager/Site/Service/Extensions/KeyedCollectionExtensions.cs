﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

// ReSharper disable once CheckNamespace

using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
    public static class KeyedCollectionExtensions
    {
        public static void AddRange<TKey, TItem>(this KeyedCollection<TKey, TItem> source, IEnumerable<TItem> items)
        where TKey : notnull
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

            foreach (TItem obj in items)
            {
                source.Add(obj);
            }
        }
    }
}
