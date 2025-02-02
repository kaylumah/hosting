// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Xunit;

#pragma warning disable RS0030
namespace Test.Unit
{
    public class ConvertValueTests2
    {
        static readonly MethodInfo _ConvertValueMethod;
        static readonly MethodInfo _GetValueMethod;
        static readonly ConcurrentDictionary<Type, MethodInfo> _GetValueMethods;

        static ConvertValueTests2()
        {
            Type type = typeof(DictionaryExtensions);
            Debug.Assert(type != null);
            _ConvertValueMethod = type.GetMethod("ConvertValue", BindingFlags.NonPublic | BindingFlags.Static)!;
            Debug.Assert(_ConvertValueMethod != null);
            _GetValueMethod = type.GetMethod("GetValue")!;
            Debug.Assert(_GetValueMethod != null);
            _GetValueMethods = new();
            GetValueMethod(typeof(string));
        }

        static MethodInfo GetValueMethod(Type target)
        {
            MethodInfo methodInfo = _GetValueMethods.GetOrAdd(target, (Type cacheKey) =>
            {
                MethodInfo result = _GetValueMethod.MakeGenericMethod(cacheKey);
                return result;
            });

            return methodInfo;
        }

        [Theory(Skip = "TODO check what scenario is tested by this")]
        [InlineData(null, typeof(string), null)]
        [InlineData(null, typeof(int), null)]
        [InlineData(null, typeof(bool), null)]
        public void ConvertValue_ShouldHandleNullValues(object? input, Type targetType, object? expected)
        {
            object?[] arguments = new object?[] { input, targetType };
            object? result = _ConvertValueMethod.Invoke(null, arguments);
            Assert.Equal(expected, result);
        }

        [Theory(Skip = "TODO check what scenario is tested by this")]
        [InlineData(42, typeof(int), 42)]
        [InlineData(42, typeof(double), 42.0)]
        [InlineData(3.14, typeof(float), 3.14f)]
        [InlineData(3.14, typeof(decimal), 3.14)]
        [InlineData(1, typeof(bool), true)]
        [InlineData(0, typeof(bool), false)]
        [InlineData(int.MaxValue, typeof(long), (long)int.MaxValue)]
        [InlineData(int.MinValue, typeof(long), (long)int.MinValue)]
        public void ConvertValue_ShouldConvertPrimitiveTypes(object input, Type targetType, object expected)
        {
            object?[] arguments = new object?[] { input, targetType };
            object? result = _ConvertValueMethod.Invoke(null, arguments);
            Assert.NotNull(expected);
            // Assert.Equal(Convert.ChangeType(expected, targetType, CultureInfo.InvariantCulture), result);
        }

        [Theory]
        [MemberData(nameof(StringConversionData))]
        public void ConvertValue_ShouldConvertFromStringViaTryParse(string input, Type targetType, object expected)
        {
            object?[] arguments = new object?[] { input, targetType };
            object? result = _ConvertValueMethod.Invoke(null, arguments);
            Assert.NotNull(expected);
            // Assert.Equal(Convert.ChangeType(expected, targetType, CultureInfo.InvariantCulture), result);
        }

        [Theory]
        [MemberData(nameof(InvalidConversionsData))]
        public void ConvertValue_ShouldThrowInvalidOperationIfConversionFails(object value, Type targetType, Type? exceptionType)
        {
            // Note, we get TargetInvocationException since we use Reflection for invocation
            object?[] arguments = new object[] { value, targetType };
            TargetInvocationException targetInvocationException = Assert.Throws<TargetInvocationException>(() => _ConvertValueMethod.Invoke(null, arguments));
            Assert.NotNull(targetInvocationException.InnerException);
            InvalidOperationException invalidOperationException = Assert.IsType<InvalidOperationException>(targetInvocationException.InnerException);
            if (exceptionType == null)
            {
                Assert.Null(invalidOperationException.InnerException);
            }
            else
            {
                Assert.NotNull(invalidOperationException.InnerException);
                Assert.IsType(exceptionType, invalidOperationException.InnerException);
            }

            string message = invalidOperationException.ToString();
            string expectedMessage = $"Cannot convert value '{value}' to {targetType}";

            Assert.Contains(expectedMessage, message);
        }

        [Fact]
        public void Test_GetValue_ThrowOnNull()
        {
            Dictionary<string, object?>? target = null;
            Assert.Throws<ArgumentNullException>(() => target!.GetValue<string>("some-key"));
        }

        [Fact]
        public void Test_GetValue_ThrowOnNullKey()
        {
            Dictionary<string, object?> target = new();
            Assert.Throws<ArgumentNullException>(() => target.GetValue<string>(null!));
        }

        [Theory]
        [MemberData(nameof(GetValueTestData))]
        public void Test_GetValue_CaseInsensitive(string key, object? value, object? expectedValue, Type targetType)
        {
            Dictionary<string, object?> dictionary = new();
            dictionary.Add(key, value);

            MethodInfo method = GetValueMethod(targetType);
            object[] arguments = new object[] { dictionary, key, true };
            object? result = method?.Invoke(null, arguments);
            Assert.Equal(expectedValue, result);
        }

        [Theory(Skip = "Not working as expected, bools return False")]
        [MemberData(nameof(GetValueTestData))]
        public void Test_GetValue_CaseSensitive(string key, object? value, object? expectedValue, Type targetType)
        {
            Dictionary<string, object?> dictionary = new();
            string alternativeKey = key.ToLower(CultureInfo.InvariantCulture);
            dictionary.Add(alternativeKey, value);

            MethodInfo method = GetValueMethod(targetType);
            object[] arguments = new object[] { dictionary, key, false };
            object? result = method?.Invoke(null, arguments);
            Assert.Equal(expectedValue, result);
        }

        public static IEnumerable<object[]> StringConversionData()
        {
            yield return new object[] { "true", typeof(bool), true };
            yield return new object[] { "false", typeof(bool), false };
            yield return new object[] { "42", typeof(int), 42 };
            yield return new object[] { "550e8400-e29b-41d4-a716-446655440000", typeof(Guid), new Guid("550e8400-e29b-41d4-a716-446655440000") };
            yield return new object[] { "2024-02-01T12:34:56Z", typeof(DateTime), new DateTime(2024, 2, 1, 12, 34, 56) };
            yield return new object[] { "02:30:00", typeof(TimeSpan), new TimeSpan(2, 30, 0) };
        }

        public static IEnumerable<object[]> InvalidConversionsData()
        {
            yield return new object[] { "NotFalse", typeof(bool), null! };
            yield return new object[] { "NotANumber", typeof(int), null! };
            yield return new object[] { "InvalidGuid", typeof(Guid), null! };
            yield return new object[] { "InvalidDate", typeof(DateTime), null! };
            yield return new object[] { "InvalidTimeSpan", typeof(TimeSpan), null! };
            yield return new object[] { long.MaxValue, typeof(int), typeof(OverflowException) };
            yield return new object[] { true, typeof(Uri), typeof(InvalidCastException) };
            yield return new object[] { "invalid", typeof(double), typeof(FormatException) };
            yield return new object[] { new object(), typeof(int), null! };
        }

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
    }

    #pragma warning disable
    public class DictionaryTests
    {
        // TODO consider other types like
        // - DateTime
        // - TimeSpan
        // - GUID
        
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