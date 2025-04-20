// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Extensions.Data.Abstractions;
using Kaylumah.Ssg.Extensions.Data.Csv;
using Kaylumah.Ssg.Extensions.Data.Json;
using Kaylumah.Ssg.Extensions.Data.Yaml;
using Xunit;

namespace Test.Unit
{
    public class ParserTests
    {
        [Fact]
        public void Test1()
        {
            ICollectionParser csvParser = new CsvParser();
        }
        
        [Fact]
        public void Test_YamlParser_1()
        {
            IYamlParser yamlParser = new YamlParser();

            string input = string.Empty; // returns null
            input = "foo: bar\n- invalidIndent"; // YamlDotNet.Core.YamlException
            object result = yamlParser.Parse<object>(input);
            // dictionary<string, object> // dictionary<string, object?>
            // DTO
            // Extra Properties (Ignored)
            // (e.g., string instead of int)
        }
        
        [Fact]
        public void Test3()
        {
            IParser jsonParser = new JsonParser();
        }
        
        /*
         * [Fact]
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
         */
    }
}