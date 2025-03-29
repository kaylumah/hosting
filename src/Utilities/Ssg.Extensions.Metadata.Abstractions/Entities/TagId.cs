// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Metadata.Abstractions
{
    public readonly record struct TagId(string Value)
    {
        public static implicit operator string(TagId tagId) => tagId.Value;
        public static implicit operator TagId(string value) => new(value);
    }
}