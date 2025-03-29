// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class AuthorMetaDataCollection : KeyedCollection<AuthorId, AuthorMetaData>
    {
        protected override AuthorId GetKeyForItem(AuthorMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<AuthorId, AuthorMetaData> Dictionary => base.Dictionary!;

        public IEnumerable<AuthorId> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<AuthorId>();

    }
}