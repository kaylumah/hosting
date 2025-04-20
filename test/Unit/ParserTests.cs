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
    }
}