// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public readonly struct AuthorId
    {
        readonly string _AuthorId;

        public AuthorId(string authorId)
        {
            _AuthorId = authorId;
        }

        public static implicit operator string(AuthorId author) => author._AuthorId;
        public static implicit operator AuthorId(string value) => new AuthorId(value);
    }

    [DebuggerDisplay("AuthorMetaData '{FullName}'")]
    public class AuthorMetaData
    {
        public AuthorId Id
        { get; set; }
        public string FullName
        { get; set; }
        public string Email
        { get; set; }
        public string Uri
        { get; set; }
        public string Picture
        { get; set; }
        public Links Links
        { get; set; } = new();
    }

    public class AuthorMetaDataCollection : KeyedCollection<AuthorId, AuthorMetaData>
    {
        protected override AuthorId GetKeyForItem(AuthorMetaData item)
        {
            return item.Id;
        }

        public new IDictionary<AuthorId, AuthorMetaData> Dictionary => base.Dictionary;

        public IEnumerable<AuthorId> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<AuthorId>();

    }
}
