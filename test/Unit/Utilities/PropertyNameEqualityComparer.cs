// Copyright (c) Kaylumah, 2025. All rights reserved.
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

            string xName = x.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
            string yName = y.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
            bool result = string.Equals(xName, yName, StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public int GetHashCode(string obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));

            int result = obj.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .GetHashCode(StringComparison.OrdinalIgnoreCase);
            return result;
        }
    }
}
