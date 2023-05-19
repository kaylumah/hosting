// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Specflow.Helpers;

namespace Test.Specflow.Entities;

public class Author
{
    [GherkinTableHeader(0)]
    public string Id { get; set; }

    [GherkinTableHeader(1)]
    public string Name { get; set; }

    [GherkinTableHeader(2)]
    public string Email { get; set; }

    [GherkinTableHeader(3)]
    public string Uri { get; set; }

    [GherkinTableHeader(4)]
    public string Picture { get; set; }
}

public class AuthorCollection : MockEntityCollection<string, Author>
{
    public override string BuildKey(Author item)
    {
        return item.Id;
    }
}
