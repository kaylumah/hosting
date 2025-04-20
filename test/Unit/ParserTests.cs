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
#pragma warning disable
    public class YamlParserTests
    {
        private readonly IYamlParser _parser = new YamlParser();

        public class TestDto
        {
            public string? Title { get; set; }
            public int Count { get; set; }
        }

        [Fact]
        public void Parse_EmptyString_ReturnsNull()
        {
            string input = """

                           """;
            object? result = _parser.Parse<object>(input);
            Assert.Null(result);
        }

        [Fact]
        public void Parse_NullString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _parser.Parse<object>(null!));
        }

        [Fact]
        public void Parse_YamlMapping_AsObject_ReturnsDictionaryObjectObject()
        {
            string input = """
                           title: doc1
                           """;
            object result = _parser.Parse<object>(input);
            Assert.IsType<Dictionary<object, object>>(result);
        }

        [Fact]
        public void Parse_YamlMapping_AsStringObjectDictionary_ReturnsTypedDictionary()
        {
            string input = """
                           title: doc1
                           """;
            var result = _parser.Parse<Dictionary<string, object>>(input);
            Assert.Equal("doc1", result["title"]);
        }

        [Fact]
        public void Parse_YamlMapping_AsDto_ReturnsPopulatedDto()
        {
            string input = """
                           title: doc1
                           count: 42
                           """;
            var result = _parser.Parse<TestDto>(input);
            Assert.Equal("doc1", result.Title);
            Assert.Equal(42, result.Count);
        }

        [Fact]
        public void Parse_YamlWithExtraProperty_IgnoresExtraAndParsesDto()
        {
            string input = """
                           title: doc1
                           count: 1
                           extra: ignored
                           """;
            var result = _parser.Parse<TestDto>(input);
            Assert.Equal("doc1", result.Title);
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void Parse_YamlWithIndentError_ThrowsSyntaxError()
        {
            string input = """
                           key1: value1
                             key2: 123
                           """;
            Assert.Throws<YamlDotNet.Core.SyntaxErrorException>(() => _parser.Parse<object>(input));
        }

        [Fact]
        public void Parse_JsonObject_AsDictionary_ReturnsStringObjectDictionary()
        {
            string input = """
                           {
                             "title": "json-doc",
                             "count": 5
                           }
                           """;
            var result = _parser.Parse<Dictionary<string, object>>(input);
            Assert.Equal("json-doc", result["title"]);
            Assert.Equal(5, Convert.ToInt32(result["count"]));
        }

        [Fact]
        public void Parse_JsonObject_AsDto_ReturnsPopulatedDto()
        {
            string input = """
                           {
                             "title": "json-dto",
                             "count": 10
                           }
                           """;
            var result = _parser.Parse<TestDto>(input);
            Assert.Equal("json-dto", result.Title);
            Assert.Equal(10, result.Count);
        }

        [Fact]
        public void Parse_InvalidJson_TrailingComma_ThrowsSyntaxError()
        {
            string input = """
                           {
                             "title": "bad json",
                           }
                           """;
            Assert.Throws<YamlDotNet.Core.SyntaxErrorException>(() => _parser.Parse<object>(input));
        }

        [Fact]
        public void Parse_JsonStringScalar_ReturnsRawString()
        {
            string input = """
                           "hello"
                           """;
            var result = _parser.Parse<string>(input);
            Assert.Equal("hello", result);
        }

        [Fact]
        public void Parse_JsonArray_ReturnsIntegerList()
        {
            string input = """
                           [1, 2, 3]
                           """;
            var result = _parser.Parse<List<int>>(input);
            Assert.Equal(new[] { 1, 2, 3 }, result);
        }

        [Fact]
        public void Parse_EmptyObjectMap_AsStringObjectDictionary_ReturnsEmpty()
        {
            string input = """
                           {}
                           """;
            var result = _parser.Parse<Dictionary<string, object>>(input);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_YamlWithExplicitNullValue_ReturnsNullInDictionary()
        {
            string input = """
                           title: null
                           """;
            var result = _parser.Parse<Dictionary<string, object?>>(input);
            Assert.Null(result["title"]);
        }

        [Fact]
        public void Parse_YamlTypeMismatch_ThrowsDeserializationException()
        {
            string input = """
                           title: abc
                           count: not-a-number
                           """;
            Assert.Throws<YamlDotNet.Core.YamlException>(() => _parser.Parse<TestDto>(input));
        }
    }

    public class JsonParserTests
    {
        private readonly IJsonParser _parser = new JsonParser();

        public class TestDto
        {
            public string? Title { get; set; }
            public int Count { get; set; }
        }

        [Fact]
        public void Parse_EmptyString_ThrowsJsonException()
        {
            string input = """

                           """;
            Assert.Throws<JsonException>(() => _parser.Parse<object>(input));
        }

        [Fact]
        public void Parse_NullInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _parser.Parse<object>(null!));
        }

        [Fact]
        public void Parse_ObjectJson_ReturnsDictionaryObject()
        {
            string input = """
                           {
                             "title": "doc1",
                             "count": 42
                           }
                           """;
            var result = _parser.Parse<Dictionary<string, object>>(input);
            Assert.Equal("doc1", result["title"]?.ToString());
            Assert.Equal(42, Convert.ToInt32(result["count"]));
        }

        [Fact]
        public void Parse_ObjectJson_ReturnsDto()
        {
            string input = """
                           {
                             "title": "doc1",
                             "count": 42
                           }
                           """;
            var result = _parser.Parse<TestDto>(input);
            Assert.Equal("doc1", result.Title);
            Assert.Equal(42, result.Count);
        }

        [Fact]
        public void Parse_EmptyObject_ReturnsEmptyDictionary()
        {
            string input = """
                           {}
                           """;
            var result = _parser.Parse<Dictionary<string, object>>(input);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_NullValue_ReturnsNullInDictionary()
        {
            string input = """
                           {
                             "title": null
                           }
                           """;
            var result = _parser.Parse<Dictionary<string, object?>>(input);
            Assert.Null(result["title"]);
        }

        [Fact]
        public void Parse_StringScalar_ReturnsString()
        {
            string input = """
                           "hello"
                           """;
            var result = _parser.Parse<string>(input);
            Assert.Equal("hello", result);
        }

        [Fact]
        public void Parse_ArrayJson_ReturnsList()
        {
            string input = """
                           [1, 2, 3]
                           """;
            var result = _parser.Parse<List<int>>(input);
            Assert.Equal(new[] { 1, 2, 3 }, result);
        }

        [Fact]
        public void Parse_TrailingComma_ThrowsJsonException()
        {
            string input = """
                           {
                             "title": "oops",
                           }
                           """;
            Assert.Throws<JsonException>(() => _parser.Parse<object>(input));
        }

        [Fact]
        public void Parse_TypeMismatch_ThrowsJsonException()
        {
            string input = """
                           {
                             "title": "abc",
                             "count": "not-a-number"
                           }
                           """;
            Assert.Throws<JsonException>(() => _parser.Parse<TestDto>(input));
        }
    }

    public class CsvParserTests
    {
        private readonly ICsvParser _parser = new CsvParser();

        public class TestDto
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }

        [Fact]
        public void Parse_EmptyString_ReturnsEmptyArray()
        {
            string input = """

                           """;
            var result = _parser.Parse<TestDto>(input);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_HeaderOnly_ReturnsEmptyArray()
        {
            string input = """
                           Name;Age
                           """;
            var result = _parser.Parse<TestDto>(input);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_SingleRowDto_ReturnsRecord()
        {
            string input = """
                           Name;Age
                           Alice;30
                           """;
            var result = _parser.Parse<TestDto>(input);

            Assert.Single(result);
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal(30, result[0].Age);
        }

        [Fact]
        public void Parse_MultipleRowsDto_ReturnsAllRecords()
        {
            string input = """
                           Name;Age
                           Bob;25
                           Eve;40
                           """;
            var result = _parser.Parse<TestDto>(input);

            Assert.Equal(2, result.Length);
            Assert.Equal("Bob", result[0].Name);
            Assert.Equal("Eve", result[1].Name);
        }

        [Fact]
        public void Parse_SingleRowDictionary_ReturnsRecord()
        {
            string input = """
                           Name;Age
                           Charlie;28
                           """;
            var result = _parser.Parse<Dictionary<string, object>>(input);

            Assert.Single(result);
            Assert.Equal("Charlie", result[0]["Name"]);
            Assert.Equal("28", result[0]["Age"]);
        }

        [Fact]
        public void Parse_PartialRow_DictionaryHandlesNulls()
        {
            string input = """
                           Name;Age
                           Diana;
                           """;
            var result = _parser.Parse<Dictionary<string, object>>(input);

            Assert.Single(result);
            Assert.Equal("Diana", result[0]["Name"]);
            Assert.Null(result[0]["Age"]);
        }

        [Fact]
        public void Parse_InvalidRow_SkippedOrHandledGracefully()
        {
            string input = """
                           Name;Age
                           Ethan;invalid
                           """;
            var result = _parser.Parse<TestDto>(input);

            // Expect 0 records because 'invalid' cannot be parsed as int
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_MissingHeader_ThrowsOrReturnsEmpty()
        {
            string input = """
                           Alice;30
                           """;
            var result = _parser.Parse<TestDto>(input);

            // Since HasHeaderRecord = true, this row will be skipped
            Assert.Empty(result);
        }
    }

    public class ParserTests
    {
        [Fact]
        public void Test1()
        {
            ICollectionParser csvParser = new CsvParser();
        }

        // TODO NULL string
        // TODO DTO Parsing

        [Fact]
        public void Test_YamlParser_EmptyString()
        {
            IYamlParser yamlParser = new YamlParser();
            string input = """

                           """;
            object result = yamlParser.Parse<object>(input);
            Assert.Null(result);
        }

        [Fact]
        public void Test_YamlParser_AsObjectReturnsDictionary()
        {
            IYamlParser yamlParser = new YamlParser();
            string input = """
                           title: doc1
                           """;
            object result = yamlParser.Parse<object>(input);
            Assert.IsType<Dictionary<object, object>>(result);
        }

        [Fact]
        public void Test_YamlParser_ExplicitReturnsDictionary()
        {
            IYamlParser yamlParser = new YamlParser();
            string input = """
                           title: doc1
                           """;
            Dictionary<string, object> result = yamlParser.Parse<Dictionary<string, object>>(input);
            Assert.IsType<Dictionary<string, object>>(result);
        }

        [Fact]
        public void Test_YamlParser_IdentThrows()
        {
            // Does not throw?
            IYamlParser yamlParser = new YamlParser();
            string input = """
                           key1: value1
                            key2: 123
                           """;
            // object result = yamlParser.Parse<object>(input);
            input = "{}";
            Assert.Throws<SemanticErrorException>(() => yamlParser.Parse<object>(input));
        }

        // Pass {} [] "Hello World"
        // Valid JSON will work
        // Invalid JSON will not { title: "missing-quotes" }

        [Fact]
        public void Test3()
        {
            IParser jsonParser = new JsonParser();
        }
    }
}