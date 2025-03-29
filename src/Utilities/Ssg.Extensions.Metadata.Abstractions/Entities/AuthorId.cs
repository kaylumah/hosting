// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Metadata.Abstractions
{
    public readonly record struct AuthorId(string Value)
    {
        public static implicit operator string(AuthorId authorId) => authorId.Value;
        public static implicit operator AuthorId(string value) => new(value);
    }
}