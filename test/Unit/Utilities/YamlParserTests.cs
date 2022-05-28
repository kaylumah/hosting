// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using FluentAssertions;
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
