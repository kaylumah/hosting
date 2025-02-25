﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test.Unit.Entities
{
    public abstract class MockEntityCollection<TKey, TItem> : KeyedCollection<TKey, TItem> where TKey : notnull
    {
        public void AddRange(IEnumerable<TItem> items)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

            foreach (TItem obj in items)
            {
                Add(obj);
            }
        }

        public abstract TKey BuildKey(TItem item);

        protected override TKey GetKeyForItem(TItem item)
        {
            TKey result = BuildKey(item);
            return result;
        }
    }
}
