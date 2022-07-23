// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Specflow.Helpers;

namespace Test.Specflow.Entities;

public class Author
{
    [GherkinTableHeader(0)]
    public string Id { get; set; }
}

public class AuthorCollection : MockEntityCollection<string, Author>
{
    public override string BuildKey(Author item)
    {
        return item.Id;
    }
}
