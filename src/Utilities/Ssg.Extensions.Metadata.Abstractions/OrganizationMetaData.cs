// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("OrganizationMetaData '{FullName}'")]
    public class OrganizationMetaData
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Linkedin { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public DateTimeOffset Founded { get; set; }
    }

    public class OrganizationMetaDataCollection : KeyedCollection<string, OrganizationMetaData>
    {
        protected override string GetKeyForItem(OrganizationMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<string, OrganizationMetaData> Dictionary => base.Dictionary!;

        public IEnumerable<string> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<string>();

    }
}
