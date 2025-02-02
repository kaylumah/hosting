// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Test.Unit
{
    public class DictionaryTests
    {
        // Int
        // String
        // Bool

        // NULL Value
        // Non-existing value
        // int value etc as string

        // List<string>, List<object>
        // Array<string> ..

        public static IEnumerable<object[]> GetValueTestData()
        {
            yield return new object[] { "stringValue", "Hello World", "Hello World", typeof(string) };
            yield return new object[] { "intValue", 42, 42, typeof(int) };
            yield return new object[] { "boolTrueValue", true, true, typeof(bool) };
            yield return new object[] { "boolFalseValue", false, false, typeof(bool) };
            yield return new object[] { "intAsStringValue", "42", 42, typeof(int) };
            yield return new object[] { "boolTrueAsStringValue", "true", true, typeof(bool) };
        }

        [Theory]
        [MemberData(nameof(GetValueTestData))]
        public void Test_GetValue(string key, object? value, object? expectedValue, Type targetType)
        {
            Dictionary<string, object?> dictionary = new();
            dictionary.Add(key, value);

            MethodInfo? method = typeof(DictionaryExtensions).GetMethod("GetValue")?.MakeGenericMethod(targetType);
            object[] arguments = new object[] { dictionary, key };
            object? result = method?.Invoke(null, arguments);
            Assert.Equal(expectedValue, result);
        }

        /*
        [Fact]
        public void Test1()
        {
            Dictionary<string, object?> original = new Dictionary<string, object?>
            {
                { "TagsAsStringList", new List<string>() { "a", "b" } },
                { "TagsAsStringArray", new [] { "c", "d" } },
                { "TagsAsObjectList", new List<object>() { "e", "f" } }
            };
            Assert.Equal(original["TagsAsStringList"].GetType(), typeof(List<string>));
            Assert.Equal(original["TagsAsStringArray"].GetType(), typeof(string[]));
            Assert.Equal(original["TagsAsObjectList"].GetType(), typeof(List<object>));
            
            YamlDotNet.Serialization.ISerializer serializer = new YamlDotNet.Serialization.SerializerBuilder().Build();
            YamlDotNet.Serialization.IDeserializer deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
            string yaml = serializer.Serialize(original);
            Dictionary<string, object?> deserialized = deserializer.Deserialize<Dictionary<string, object?>>(yaml);
            Assert.Equal(deserialized["TagsAsStringList"].GetType(), typeof(List<object>));
            Assert.Equal(deserialized["TagsAsStringArray"].GetType(), typeof(List<object>));
            Assert.Equal(deserialized["TagsAsObjectList"].GetType(), typeof(List<object>));
        }
        */
    }
}