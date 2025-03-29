// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Metadata.Abstractions
{
    public readonly record struct PageId(string Value)
    {
        public static implicit operator string(PageId pageId) => pageId.Value;
        public static implicit operator PageId(string value) => new(value);
    }
}