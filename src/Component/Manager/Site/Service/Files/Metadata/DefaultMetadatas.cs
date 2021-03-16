// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class DefaultMetadatas : KeyedCollection<string, DefaultMetadata>
    {
        protected override string GetKeyForItem(DefaultMetadata item)
        {
            if (item.Scope != null)
            {
                return $"{item.Path}.{item.Scope}";
            }
            return item.Path;
        }
    }
}