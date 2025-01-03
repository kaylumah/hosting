﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Test.Unit.Helpers
{
    public class PropertyNameEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            return string.Equals(x.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase),
                y.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));

            return obj.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .GetHashCode(StringComparison.OrdinalIgnoreCase);
        }
    }
}
