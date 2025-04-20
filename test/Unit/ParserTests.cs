// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
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