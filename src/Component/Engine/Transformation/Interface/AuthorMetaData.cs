// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

[DebuggerDisplay("AuthorMetaData '{FullName}'")]
public class AuthorMetaData
{
    public string Id { get;set; }
    public string FullName { get;set; }
    public string Email { get;set; }
    public string Uri { get;set; }
    public string Picture { get;set; }
    public Links Links { get;set; } = new();
}

public class AuthorMetaDataCollection : KeyedCollection<string, AuthorMetaData>
{
    protected override string GetKeyForItem(AuthorMetaData item)
    {
        return item.Id;
    }

    public new IDictionary<string, AuthorMetaData> Dictionary => base.Dictionary;

    public IEnumerable<string> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<string>();

}
