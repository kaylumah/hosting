// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

[DebuggerDisplay("OrganizationMetaData '{FullName}'")]
public class OrganizationMetaData
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Linkedin { get; set; }
    public string Twitter { get; set; }
    public string Logo { get; set; }
    public DateTimeOffset Founded { get; set; }
}

public class OrganizationMetaDataCollection : KeyedCollection<string, OrganizationMetaData>
{
    protected override string GetKeyForItem(OrganizationMetaData item)
    {
        return item.Id;
    }

    public new IDictionary<string, OrganizationMetaData> Dictionary => base.Dictionary;

    public IEnumerable<string> Keys => base.Dictionary?.Keys ?? Enumerable.Empty<string>();

}
