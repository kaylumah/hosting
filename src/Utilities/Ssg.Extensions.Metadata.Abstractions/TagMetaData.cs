// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("TagMetaData '{Name}'")]
    public class TagMetaData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

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
