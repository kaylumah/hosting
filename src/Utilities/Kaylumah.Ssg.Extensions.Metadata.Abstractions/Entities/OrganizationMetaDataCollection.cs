// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
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