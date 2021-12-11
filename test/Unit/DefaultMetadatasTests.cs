// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Xunit;

namespace Test.Unit;

public class DefaultMetadatasTests
{
    [Fact]
    public void TestKey()
    {
        var itemWithoutScope = new DefaultMetadata() { Path = "" };
        var itemWithScope = new DefaultMetadata() { Path = "", Scope = "" };
        var itemWithNamedScope = new DefaultMetadata() { Path = "", Scope = "posts" };
        var itemPathWithNameScope = new DefaultMetadata() { Path = "2019", Scope = "posts" };

        var data = new DefaultMetadatas
            {
                itemWithoutScope,
                itemWithScope,
                itemWithNamedScope,
                itemPathWithNameScope
            };

        data[""].Should().NotBeNull();
        data["."].Should().NotBeNull();
        data[".posts"].Should().NotBeNull();
        data["2019.posts"].Should().NotBeNull();
    }
}