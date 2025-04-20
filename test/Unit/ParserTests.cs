// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using Kaylumah.Ssg.Extensions.Data.Abstractions;
using IParser = Kaylumah.Ssg.Extensions.Data.Abstractions.IParser;
using Kaylumah.Ssg.Extensions.Data.Csv;
using Kaylumah.Ssg.Extensions.Data.Json;
using Kaylumah.Ssg.Extensions.Data.Yaml;
using Xunit;
using YamlDotNet.Core;

namespace Test.Unit
{
    public class ParserTests
    {
        internal sealed class TestDto
        {
            public string? Name
            { get; set; }

            public int? Age
            { get; set; }
        }

        readonly ICsvParser _CsvParser;
        readonly IJsonParser _JsonParser;
        readonly IYamlParser _YamlParser;

        public ParserTests()
        {
            _CsvParser = new CsvParser();
            _JsonParser = new JsonParser();
            _YamlParser = new YamlParser();

            // TODO consider test cases
            // - CSV without Header, header only
            // - CSV, JSON, Yaml invalid datatypes
            // - CSV, JSON, Yaml Nullability
            // object
            // dictionary<string, object?>
            // Dto
        }

        [Fact]
        public void Test_CsvParser_HandlesNullValue()
        {
            string input = null!;
            Assert.Throws<ArgumentNullException>(() => _CsvParser.Parse<object>(input));
        }

        [Fact]
        public void Test_CsvParser_HandlesEmptyString()
        {
            string input = string.Empty;
            Assert.Throws<ArgumentException>(() => _CsvParser.Parse<object>(input));
        }

        [Fact]
        public void Test_CsvParser_HandlesWhitespaceString()
        {
            string input = "   ";
            Assert.Throws<ArgumentException>(() => _CsvParser.Parse<object>(input));
        }

        [Fact]
        public void Test_CsvParser_HandlesObject()
        {
            string input = """
                           Name;Age;
                           Max;30;
                           """;
            object result = _CsvParser.Parse<object>(input);
        }

        [Fact]
        public void Test_CsvParser_HandlesDictionary()
        {
            string input = """
                           Name;Age;
                           Max;30;
                           """;
            Dictionary<string, object>[] result = _CsvParser.Parse<Dictionary<string, object>>(input);
        }

        [Fact]
        public void Test_CsvParser_HandlesDto()
        {
            string input = """
                           Name;Age;
                           Max;30;
                           """;
            TestDto[] result = _CsvParser.Parse<TestDto>(input);
        }

        [Fact]
        public void Test_JsonParser_HandlesNullValue()
        {
            string input = null!;
            Assert.Throws<ArgumentNullException>(() => _JsonParser.Parse<object>(input));
        }

        [Fact]
        public void Test_JsonParser_HandlesEmptyString()
        {
            string input = string.Empty;
            Assert.Throws<ArgumentException>(() => _JsonParser.Parse<object>(input));
        }

        [Fact]
        public void Test_JsonParser_HandlesWhitespaceString()
        {
            string input = "   ";
            Assert.Throws<ArgumentException>(() => _JsonParser.Parse<object>(input));
        }

        [Fact]
        public void Test_JsonParser_HandlesObject()
        {
            string input = """
                           {
                            "Name": "max",
                            "Age": 30
                           }
                           """;
            object result = _JsonParser.Parse<object>(input);
            Assert.IsType<JsonElement>(result);
            if (result is JsonElement jsonElement)
            {
#pragma warning disable
                Assert.Equal("max", jsonElement.GetProperty("Name").GetString());
                Assert.Equal(30, jsonElement.GetProperty("Age").GetInt32());
#pragma warning restore
            }
        }

        [Fact]
        public void Test_JsonParser_HandlesDictionary()
        {
            string input = """
                           {
                            "Name": "max",
                            "Age": 30
                           }
                           """;
            Dictionary<string, object> result = _JsonParser.Parse<Dictionary<string, object>>(input);
            Assert.True(result["Name"] is JsonElement);
            Assert.True(result["Age"] is JsonElement);

            string? name = ((JsonElement)result["Name"]).GetString();
            int age = ((JsonElement)result["Age"]).GetInt32();

            Assert.Equal("max", name);
            Assert.Equal(30, age);
        }

        [Fact]
        public void Test_JsonParser_HandlesDto()
        {
            string input = """
                           {
                            "Name": "max",
                            "Age": 30
                           }
                           """;
            TestDto result = _JsonParser.Parse<TestDto>(input);
            Assert.Equal("max", result.Name);
            Assert.Equal(30, result.Age);
        }

        [Fact]
        public void Test_YamlParser_HandlesNullValue()
        {
            string input = null!;
            Assert.Throws<ArgumentNullException>(() => _YamlParser.Parse<object>(input));
        }

        [Fact]
        public void Test_YamlParser_HandlesEmptyString()
        {
            string input = string.Empty;
            Assert.Throws<ArgumentException>(() => _YamlParser.Parse<object>(input));
        }

        [Fact]
        public void Test_YamlParser_HandlesWhitespaceString()
        {
            string input = "   ";
            Assert.Throws<ArgumentException>(() => _YamlParser.Parse<object>(input));
        }

        [Fact]
        public void Test_YamlParser_HandlesObject()
        {
            string input = """
                           name: max
                           age: 30
                           """;
            object result = _YamlParser.Parse<object>(input);
            Assert.IsType<Dictionary<object, object>>(result);
            if (result is Dictionary<object, object> dictionary)
            {
                Assert.Equal("max", dictionary["name"]);
                Assert.Equal("30", dictionary["age"]);
            }
        }

        [Fact]
        public void Test_YamlParser_HandlesDictionary()
        {
            string input = """
                           name: max
                           age: 30
                           """;
            Dictionary<string, object> result = _YamlParser.Parse<Dictionary<string, object>>(input);
            Assert.Equal("max", result["name"]);
            Assert.Equal("30", result["age"]);
        }

        [Fact]
        public void Test_YamlParser_HandlesDto()
        {
            string input = """
                           name: max
                           age: 30
                           """;
            TestDto result = _YamlParser.Parse<TestDto>(input);
            Assert.Equal("max", result.Name);
            Assert.Equal(30, result.Age);
        }
    }
}