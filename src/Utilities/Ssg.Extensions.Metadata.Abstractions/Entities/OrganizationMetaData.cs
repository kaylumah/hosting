// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public readonly struct OrganizationId
    {
        readonly string _OrganizationId;

        public OrganizationId(string organizationId)
        {
            _OrganizationId = organizationId;
        }

        public static implicit operator string(OrganizationId organization) => organization._OrganizationId;
        public static implicit operator OrganizationId(string value) => new OrganizationId(value);
    }

    [DebuggerDisplay("OrganizationMetaData '{FullName}'")]
    public class OrganizationMetaData
    {
        public OrganizationId Id
        { get; set; }
        public string FullName
        { get; set; }
        public string Linkedin
        { get; set; }
        public string Twitter
        { get; set; }
        public string Logo
        { get; set; }
        public DateTimeOffset Founded
        { get; set; }
    }

    public class OrganizationMetaDataCollection : KeyedCollection<OrganizationId, OrganizationMetaData>
    {
        protected override OrganizationId GetKeyForItem(OrganizationMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<OrganizationId, OrganizationMetaData> Dictionary => base.Dictionary;

        public IEnumerable<OrganizationId> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<OrganizationId>();

    }
}
