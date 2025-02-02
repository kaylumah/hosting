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
        private readonly Dictionary<string, object?> _dictionary = new()
    {
        { "intValue", 42 },
        { "boolValue", true },
        { "stringValue", "Hello" },
        { "guidValue", "550e8400-e29b-41d4-a716-446655440000" },
        { "dateTimeValue", "2024-02-01T12:34:56Z" },
        { "timeSpanValue", "02:30:00" },
        { "stringInt", "42" },
        { "stringBool", "true" },
        { "intList", new List<object> { 1, 2, 3 } },
        { "stringList", new List<object> { "a", "b", "c" } },
        { "intArray", new object[] { 4, 5, 6 } },
        { "stringArray", new object[] { "x", "y", "z" } },
        { "nullValue", null },
        { "invalidInt", "notAnInt" },
        { "stringWithSpaces", "  42  " },
        { "mixedCaseBool", "TrUe" },
        { "emptyList", new List<object>() },
        { "mixedList", new List<object> { 1, "2", "three" } },
        { "CaseSensitiveKey", "Hello" },
        { "listWithNull", new List<object> { 1, null, 3 } },
        { "directIntList", new List<int> { 10, 20, 30 } },
        { "directStringList", new List<string> { "alpha", "beta", "gamma" } },
        { "stringObjectArray", new object[] { "p", "q", "r" } },
        { "maxInt", int.MaxValue },
        { "minInt", int.MinValue },
        { "maxLong", long.MaxValue },
        { "nullableStringList", new List<object> { "hello", null, "world" } },
        { "nullList", null },
        { "stringNumberList", new List<string> { "10", "20", "30" } }
    };

        public static IEnumerable<object[]> GetValueTestData()
        {
            yield return new object[] { "intValue", 42, typeof(int) };
            yield return new object[] { "boolValue", true, typeof(bool) };
            yield return new object[] { "stringValue", "Hello", typeof(string) };
            yield return new object[] { "stringInt", 42, typeof(int) };
            yield return new object[] { "stringBool", true, typeof(bool) };
            yield return new object[] { "guidValue", Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), typeof(Guid) };
            yield return new object[] { "dateTimeValue", DateTime.Parse("2024-02-01T12:34:56Z", null, System.Globalization.DateTimeStyles.RoundtripKind), typeof(DateTime) };
            yield return new object[] { "timeSpanValue", TimeSpan.Parse("02:30:00"), typeof(TimeSpan) };
            yield return new object[] { "maxInt", int.MaxValue, typeof(int) };
            yield return new object[] { "minInt", int.MinValue, typeof(int) };
        }

        [Theory]
        [MemberData(nameof(GetValueTestData))]
        public void GetValue_ShouldReturnCorrectValue(string key, object expectedValue, Type targetType)
        {
            var method = typeof(DictionaryExtensions).GetMethod("GetValue").MakeGenericMethod(targetType);
            var result = method.Invoke(_dictionary, new object[] { _dictionary, key, true });
            Assert.Equal(expectedValue, result);
        }

        public static IEnumerable<object[]> GetListTestData()
        {
            yield return new object[] { "directIntList", new List<int> { 10, 20, 30 } };
            yield return new object[] { "directStringList", new List<string> { "alpha", "beta", "gamma" } };
            yield return new object[] { "stringObjectArray", new List<string> { "p", "q", "r" } };
            yield return new object[] { "stringNumberList", new List<int> { 10, 20, 30 } };
        }

        [Theory]
        [MemberData(nameof(GetListTestData))]
        public void GetValues_ShouldReturnCorrectList(string key, object expectedList)
        {
            var result = _dictionary.GetValues<object>(key);
            Assert.Equal(expectedList, result);
        }

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
        }


        // Int
        // String
        // Bool

        // NULL Value
        // Non-existing value
        // int value etc as string

        // List<string>, List<object>
        // Array<string> ..

        /*
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
        public void Test_GetValue_CaseInsensitive(string key, object? value, object? expectedValue, Type targetType)
        {
            Dictionary<string, object?> dictionary = new();
            dictionary.Add(key, value);

            MethodInfo? method = typeof(DictionaryExtensions).GetMethod("GetValue")?.MakeGenericMethod(targetType);
            object[] arguments = new object[] { dictionary, key, true };
            object? result = method?.Invoke(null, arguments);
            Assert.Equal(expectedValue, result);
        }
        */

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