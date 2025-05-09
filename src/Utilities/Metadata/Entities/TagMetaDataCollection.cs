// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public class TagMetaDataCollection : KeyedCollection<string, TagMetaData>
    {
        protected override string GetKeyForItem(TagMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<string, TagMetaData> Dictionary => base.Dictionary!;

        public IEnumerable<string> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<string>();

    }
}