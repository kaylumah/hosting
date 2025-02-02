// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
#pragma warning disable
namespace Test.Unit
{
    public class DictionaryTests
    {
        // TODO consider other types like
        // - DateTime
        // - TimeSpan
        // - GUID

        public static IEnumerable<object[]> GetValueTestData()
        {
            // TODO List?
            // TODO NULL Value
            // TODO Invalid INT (string? notAnInt)
            // TODO Invalid Bool
            // TODO string with Spaces //"  42  "
            // TODO mixed case bool // "TrUe" 
            // int.Max - int.MinValue
            yield return new object[] { "stringValue", "Hello World", "Hello World", typeof(string) };
            yield return new object[] { "intValue", 42, 42, typeof(int) };
            yield return new object[] { "boolTrueValue", true, true, typeof(bool) };
            yield return new object[] { "boolFalseValue", false, false, typeof(bool) };
            yield return new object[] { "intAsStringValue", "42", 42, typeof(int) };
            yield return new object[] { "boolTrueAsStringValue", "true", true, typeof(bool) };
            yield return new object[] { "boolTrueAsStringValue", "false", false, typeof(bool) };
        }

        public static IEnumerable<object[]> GetValuesTestData()
        {
            // TODO bool list
            // TODO int list
            // TODO list of string int
            // TODO empty list
            // TODO list with NULL

            yield return new object[] { "stringsAsListOfString", new List<string>() { "a", "b", "c" }, new List<string>() { "a", "b", "c" }, typeof(string) };
            yield return new object[] { "stringsAsListOfObject", new List<object>() { "a", "b", "c" }, new List<string>() { "a", "b", "c" }, typeof(string) };
            yield return new object[] { "stringsAsArrayOfString", new string[] { "a", "b", "c" }, new List<string>() { "a", "b", "c" }, typeof(string) };
            yield return new object[] { "stringsAsArrayOfObject", new object[] { "a", "b", "c" }, new List<string>() { "a", "b", "c" }, typeof(string) };
            yield return new object[] { "singleStringAsListOfString", new List<string>() { "a" }, new List<string>() { "a" }, typeof(string) };
            yield return new object[] { "singleStringAsListOfObject", new List<object>() { "a" }, new List<string>() { "a" }, typeof(string) };
            yield return new object[] { "singleStringsAsArrayOfString", new string[] { "a" }, new List<string>() { "a" }, typeof(string) };
            yield return new object[] { "string", "a", new List<string>() { "a" }, typeof(string) };
        }


        // TODO: GetValue where dictionary == null should throw
        // TODO: GetValue where key == null should throw
        // TODO: Test different default values
        // TODO: Check different null values

        [Theory]
        [MemberData(nameof(GetValueTestData))]
        public void Test_GetValue_CaseInsensitive(string key, object? value, object? expectedValue, Type targetType)
        {
            Dictionary<string, object?> dictionary = new();
            dictionary.Add(key, value);

            MethodInfo? method = typeof(DictionaryExtensions).GetMethod("GetValue")?.MakeGenericMethod(targetType);
            object[] arguments = new object[] { dictionary, key, true };
            object? result = method?.Invoke(null, arguments);
            Assert.Equal(expectedValue, result);
        }

        [Theory]
        [MemberData(nameof(GetValuesTestData))]
        public void Test_GetValues_CaseInsensitive(string key, object? value, object? expectedValue, Type targetType)
        {
            Type genericIEnumerable = typeof(IEnumerable<>);
            Type expectedEnumerableType = genericIEnumerable.MakeGenericType(targetType);

            Dictionary<string, object?> dictionary = new();
            dictionary.Add(key, value);

            MethodInfo? method = typeof(DictionaryExtensions).GetMethod("GetValues")?.MakeGenericMethod(targetType);
            object[] arguments = new object[] { dictionary, key, true };
            object? result = method?.Invoke(null, arguments);
            Type? actualType = result.GetType();
            bool isCorrectEnumerable = actualType.IsAssignableTo(expectedEnumerableType);
            Assert.True(isCorrectEnumerable);

            // expected list
            Assert.Equal(expectedValue, result);
        }

        /*
        [Fact]
        public void GetValue_ShouldThrowForLongMaxValueToInt()
        {
            Assert.Throws<InvalidOperationException>(() => _dictionary.GetValue<int>("maxLong"));
        }

        [Fact]
        public void GetValues_ShouldReturnEmptyListForEmptyList()
        {
            Assert.Empty(_dictionary.GetValues<int>("emptyList"));
        }

        [Fact]
        public void GetValues_ShouldThrowForMixedTypeList()
        {
            Assert.Throws<InvalidOperationException>(() => _dictionary.GetValues<int>("mixedList"));
        }

        [Fact]
        public void GetValues_ShouldAllowNullableTypesWithNullValues()
        {
            Assert.Equal(new List<string?> { "hello", null, "world" }, _dictionary.GetValues<string?>("nullableStringList"));
        }

        [Fact]
        public void GetValues_ShouldReturnNullForNullList()
        {
            Assert.Null(_dictionary.GetValues<string>("nullList"));
        }

        [Fact]
        public void GetValue_ShouldRespectCaseSensitivity()
        {
            Assert.Null(_dictionary.GetValue<string>("casesensitivekey", caseInsensitive: false));
        }*/

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