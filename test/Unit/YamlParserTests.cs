// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using FluentAssertions;
using Kaylumah.Ssg.Utilities;
using Ssg.Extensions.Data.Yaml;
using Xunit;

namespace Test.Unit;

public class YamlParserTests
{
    private readonly IYamlParser _sut;

    public YamlParserTests()
    {
        _sut = new YamlParser();
    }

    [Fact]
    public void Test_YamlParser_Parse_ReturnsNullOnEmptyInput()
    {
        var input = string.Empty;
        var result = _sut.Parse<Dictionary<string, object>>(input);
        result.Should().BeNull();
    }

    [Fact]
    public void Test_YamlParser_Parse_CanReturnDictionary()
    {
        var input = "title: doc1";
        var result = _sut.Parse<Dictionary<string, object>>(input);
        result.Should().NotBeNull();
        result.ContainsKey("title").Should().BeTrue();
        result["title"].Should().Be("doc1");
    }
}