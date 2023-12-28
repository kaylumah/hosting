// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;

namespace Test.Specflow.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GherkinTableHeaderAttribute : Attribute
    {
        public int HeaderIndex { get; }

        public GherkinTableHeaderAttribute(int headerIndex)
        {
            HeaderIndex = headerIndex;
        }
    }
}
