﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public readonly record struct OrganizationId(string Value)
    {
        public static implicit operator string(OrganizationId organizationId) => organizationId.Value;
        public static implicit operator OrganizationId(string value) => new(value);
    }

    [DebuggerDisplay("OrganizationMetaData '{FullName}'")]
    public class OrganizationMetaData
    {
        public OrganizationId Id
        { get; set; } = null!;
        public string FullName
        { get; set; } = null!;
        public string Linkedin
        { get; set; } = null!;
        public string Twitter
        { get; set; } = null!;
        public string Logo
        { get; set; } = null!;
        public DateTimeOffset Founded
        { get; set; }
    }

    public class OrganizationMetaDataCollection : KeyedCollection<OrganizationId, OrganizationMetaData>
    {
        protected override OrganizationId GetKeyForItem(OrganizationMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<OrganizationId, OrganizationMetaData> Dictionary => base.Dictionary!;

        public IEnumerable<OrganizationId> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<OrganizationId>();

    }
}