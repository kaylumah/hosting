// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

public class TagMetaDataCollection : KeyedCollection<string, TagMetaData>
{
    protected override string GetKeyForItem(TagMetaData item)
    {
        return item.Id;
    }

    public new IDictionary<string, TagMetaData> Dictionary => base.Dictionary;

    public IEnumerable<string> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<string>();

}
