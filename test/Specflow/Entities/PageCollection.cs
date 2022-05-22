// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Test.Specflow.Entities;

public class PageCollection : MockEntityCollection<string, Page>
{
    public override string BuildKey(Page item)
    {
        return item.Uri;
    }
}
