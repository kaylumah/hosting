// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Test.Specflow.Entities;

public class BlogPost
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}
