// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Test.Specflow.Entities;

public class FileCollection : MockEntityCollection<string, File>
{
    public override string BuildKey(File item)
    {
        return item.Name;
    }
}
