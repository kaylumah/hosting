// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public readonly record struct SiteId(string Value)
    {
        public static implicit operator string(SiteId siteId) => siteId.Value;
        public static implicit operator SiteId(string value) => new(value);
    }
}