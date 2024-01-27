// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("AuthorMetaData '{FullName}'")]
    public class AuthorMetaData
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public Links Links { get; set; } = new();
    }

    public class AuthorMetaDataCollection : KeyedCollection<string, AuthorMetaData>
    {
        protected override string GetKeyForItem(AuthorMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<string, AuthorMetaData> Dictionary => base.Dictionary!;

        public IEnumerable<string> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<string>();

    }
}
