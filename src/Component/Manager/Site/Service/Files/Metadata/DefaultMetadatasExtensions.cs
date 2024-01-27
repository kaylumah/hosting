// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata
{
    public static partial class DefaultMetadatasExtensions
    {
        public static DefaultMetadata DefaultFilter(this DefaultMetadatas metadata, string path)
        {
            return metadata
                .Find(path, item => string.IsNullOrEmpty(item.Scope));
        }

        public static DefaultMetadata ScopeFilter(this DefaultMetadatas metadata, string path, string scope)
        {
            return metadata
                .Find(path, item => item.Scope != null && item.Scope.Equals(scope, StringComparison.Ordinal));
        }

        public static DefaultMetadata Find(this DefaultMetadatas metadata, string path, Func<DefaultMetadata, bool> predicate)
        {
            DefaultMetadata item = metadata
                .Where(x => x.Path.Equals(path, StringComparison.Ordinal))
                .SingleOrDefault(predicate)!;
            return item;
        }
    }
}
