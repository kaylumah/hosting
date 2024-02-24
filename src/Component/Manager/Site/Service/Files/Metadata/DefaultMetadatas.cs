// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata
{
    public class DefaultMetadatas : KeyedCollection<string, DefaultMetadata>
    {
        protected override string GetKeyForItem(DefaultMetadata item)
        {
            string extJoined = string.Join('|', item.Extensions);
            string extKey = extJoined.Replace(".", string.Empty);

            if (item.Scope != null)
            {
                return $"{item.Path}.{item.Scope}.{extKey}";
            }

            return $"{item.Path}.{extKey}";
        }

        public DefaultMetadata? DefaultFilter(string extension, string path)
        {
            DefaultMetadata? result = Find(extension, path, item => string.IsNullOrEmpty(item.Scope));
            return result;
        }

        public DefaultMetadata? ScopeFilter(string extension, string path, string scope)
        {
            DefaultMetadata? result = Find(extension, path, item => item.Scope != null && item.Scope.Equals(scope, StringComparison.Ordinal));
            return result;
        }

        DefaultMetadata? Find(string extension, string path, Func<DefaultMetadata, bool> predicate)
        {
            DefaultMetadata? item = this
                .Where(x => x.Extensions.Contains(extension))
                .Where(x => x.Path.Equals(path, StringComparison.Ordinal))
                .SingleOrDefault(predicate);
            return item;
        }
    }
}
