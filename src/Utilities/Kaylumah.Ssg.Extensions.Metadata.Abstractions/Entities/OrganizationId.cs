// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public readonly record struct OrganizationId(string Value)
    {
        public static implicit operator string(OrganizationId organizationId) => organizationId.Value;
        public static implicit operator OrganizationId(string value) => new(value);
    }
}