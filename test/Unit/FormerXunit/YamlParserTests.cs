// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using FluentAssertions;
using Ssg.Extensions.Data.Yaml;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class YamlParserTests
    {
        readonly IYamlParser _Sut;

        public YamlParserTests()
        {
            _Sut = new YamlParser();
        }

        [Fact]
        public void Test_YamlParser_Parse_ReturnsNullOnEmptyInput()
        {
            string input = string.Empty;
            Dictionary<string, object> result = _Sut.Parse<Dictionary<string, object>>(input);
            result.Should().BeNull();
        }

        [Fact]
        public void Test_YamlParser_Parse_CanReturnDictionary()
        {
            string input = "title: doc1";
            Dictionary<string, object> result = _Sut.Parse<Dictionary<string, object>>(input);
            result.Should().NotBeNull();
            result.ContainsKey("title").Should().BeTrue();
            result["title"].Should().Be("doc1");
        }
    }
}
