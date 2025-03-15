// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Ssg.Extensions.Metadata.Abstractions
{
    class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T? x, T? y)
        {
            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            // Reverse the comparison for descending order
            int result = y.CompareTo(x);
            return result;
        }
    }
}