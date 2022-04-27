// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;

namespace Kaylumah.Ssg.Manager.Site.Service;

public class Collections : KeyedCollection<string, Collection>
{
    protected override string GetKeyForItem(Collection item) => item.Name;
}