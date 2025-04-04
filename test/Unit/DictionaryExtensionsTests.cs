// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xunit;

#pragma warning disable RS0030
namespace Test.Unit
{
    public class DictionaryExtensionsTests
    {
        static readonly MethodInfo _ConvertValueMethod;
        static readonly MethodInfo _GetValueMethod;
        static readonly MethodInfo _GetValuesMethod;
        static readonly ConcurrentDictionary<Type, MethodInfo> _GetValueMethods;
        static readonly ConcurrentDictionary<Type, MethodInfo> _GetValuesMethods;

        static DictionaryExtensionsTests()
        {
            Type type = typeof(DictionaryExtensions);
            Debug.Assert(type != null);
            _ConvertValueMethod = type.GetMethod("ConvertValue", BindingFlags.Public | BindingFlags.Static)!;
            Debug.Assert(_ConvertValueMethod != null);
            _GetValueMethod = type.GetMethod("GetValue")!;
            Debug.Assert(_GetValueMethod != null);
            _GetValuesMethod = type.GetMethod("GetValues")!;
            Debug.Assert(_GetValuesMethod != null);

            _GetValueMethods = new();
            _GetValuesMethods = new();
            GetValueMethod(typeof(string));
            GetValuesMethod(typeof(string));
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

        static MethodInfo GetValuesMethod(Type target)
        {
            MethodInfo methodInfo = _GetValuesMethods.GetOrAdd(target, (Type cacheKey) =>
            {
                MethodInfo result = _GetValuesMethod.MakeGenericMethod(cacheKey);
                return result;
            });

            return methodInfo;
        }

        [Theory]
        [InlineData(null, typeof(string), null)]
        [InlineData(null, typeof(int), null)]
        [InlineData(null, typeof(bool), null)]
        public void ConvertValue_ShouldHandleNullValues(object? input, Type targetType, object? expected)
        {
            object?[] arguments = new object?[] { input, targetType };
            object? result = _ConvertValueMethod.Invoke(null, arguments);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(42, typeof(int), 42)]
        [InlineData(42, typeof(double), 42.0)]
        [InlineData(3.14, typeof(float), 3.14f)]
        // [InlineData(3.14, typeof(decimal), 3.14)]
        [InlineData(1, typeof(bool), true)]
        [InlineData(0, typeof(bool), false)]
        [InlineData(int.MaxValue, typeof(long), (long)int.MaxValue)]
        [InlineData(int.MinValue, typeof(long), (long)int.MinValue)]
        public void ConvertValue_ShouldConvertPrimitiveTypes(object input, Type targetType, object expected)
        {
            object?[] arguments = new object?[] { input, targetType };
            object? result = _ConvertValueMethod.Invoke(null, arguments);
            Assert.NotNull(expected);
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(StringConversionData))]
        public void ConvertValue_ShouldConvertFromStringViaTryParse(string input, Type targetType, object expected)
        {
            object?[] arguments = new object?[] { input, targetType };
            object? result = _ConvertValueMethod.Invoke(null, arguments);
            Assert.NotNull(expected);
            Assert.Equal(expected, result);
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
        public void Test_GetValue(string key, object? value, object? expectedValue, Type targetType)
        {
            Dictionary<string, object?> dictionary = new();
            dictionary.Add(key, value);

            MethodInfo method = GetValueMethod(targetType);
            object[] arguments = new object[] { dictionary, key, true };
            object? result = method?.Invoke(null, arguments);
            Assert.Equal(expectedValue, result);
        }

        [Theory]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(bool), false)]
        public void Test_GetValue_NULL_ReturnsDefault(Type targetType, object? expectedValue)
        {
            string key = "my-key";
            Dictionary<string, object?> dictionary = new()
            {
                { key, null }
            };
            MethodInfo method = GetValueMethod(targetType);
            object[] arguments = new object[] { dictionary, key, true };
            object? result = method?.Invoke(null, arguments);
            Assert.Equal(expectedValue, result);
        }

        [Theory]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(bool), false)]
        public void Test_GetValue_NonExisting_ReturnsDefault(Type targetType, object? expectedValue)
        {
            string key = "my-key";
            Dictionary<string, object?> dictionary = new()
            {
                { key, null }
            };
            MethodInfo method = GetValueMethod(targetType);
            string searchKey = key + "-fake";
            object[] arguments = new object[] { dictionary, searchKey, true };
            object? result = method?.Invoke(null, arguments);
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void Test_GetValues_ThrowOnNull()
        {
            Dictionary<string, object?>? target = null;
            Assert.Throws<ArgumentNullException>(() => target!.GetValues<string>("some-key"));
        }

        [Fact]
        public void Test_GetValues_ThrowOnNullKey()
        {
            Dictionary<string, object?> target = new();
            Assert.Throws<ArgumentNullException>(() => target.GetValues<string>(null!));
        }

        [Theory]
        [MemberData(nameof(GetEnumerableValueTestData))]
        public void Test_GetValues(string key, object? value, object? expectedValue, Type targetType)
        {
            Type genericIEnumerable = typeof(IEnumerable<>);
            Type expectedEnumerableType = genericIEnumerable.MakeGenericType(targetType);

            Dictionary<string, object?> dictionary = new();
            dictionary.Add(key, value);

            MethodInfo? method = GetValuesMethod(targetType);
            object[] arguments = new object[] { dictionary, key, true };
            object? result = method?.Invoke(null, arguments);
            Type? actualType = result?.GetType();
            bool? isCorrectEnumerable = actualType?.IsAssignableTo(expectedEnumerableType);
            Assert.True(isCorrectEnumerable);

            // expected list
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void Test_GetValues_NonExisting_ReturnsDefault()
        {
            Dictionary<string, object?> dictionary = new();
            MethodInfo? method = GetValuesMethod(typeof(string));
            object[] arguments = new object[] { dictionary, "fake-key", true };
            object? result = method?.Invoke(null, arguments);
            Assert.Null(result);
        }

        [Fact]
        public void Test_GetValues_Exception_NotIEnumerable()
        {
            Dictionary<string, object?> dictionary = new()
            {
                { "NotAList", 42 }
            };

            MethodInfo? method = GetValuesMethod(typeof(string));
            object[] arguments = new object[] { dictionary, "NotAList", true };
            TargetInvocationException targetInvocationException = Assert.Throws<TargetInvocationException>(() => method?.Invoke(null, arguments));
            Assert.NotNull(targetInvocationException.InnerException);
            InvalidOperationException invalidOperationException = Assert.IsType<InvalidOperationException>(targetInvocationException.InnerException);
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

        public static IEnumerable<object[]> GetValueTestData()
        {
            // Considerations for later
            // - GetValues supports single to List, should we offer the reverse?
            // - Values with spaces "  42  "
            // - Values with mixed-casing "TrUe"
            // - Boundary values, int.MinValue

            yield return new object[] { "stringValue", "Hello World", "Hello World", typeof(string) };
            yield return new object[] { "intValue", 42, 42, typeof(int) };
            yield return new object[] { "boolTrueValue", true, true, typeof(bool) };
            yield return new object[] { "boolFalseValue", false, false, typeof(bool) };
            yield return new object[] { "intAsStringValue", "42", 42, typeof(int) };
            yield return new object[] { "boolTrueAsStringValue", "true", true, typeof(bool) };
            yield return new object[] { "boolTrueAsStringValue", "false", false, typeof(bool) };
        }

        public static IEnumerable<object?[]> GetEnumerableValueTestData()
        {
            yield return new object?[] { "stringsEmpty", Array.Empty<string>(), Array.Empty<string>(), typeof(string) };
            // yield return new object?[] { "stringsNull", null, null, typeof(string) };
            yield return new object?[] { "stringsAsListOfString", new List<string>() { "a", "b", "c" }, new List<string>() { "a", "b", "c" }, typeof(string) };
            yield return new object?[] { "stringsAsArrayOfString", new string[] { "a", "b", "c" }, new List<string>() { "a", "b", "c" }, typeof(string) };
            yield return new object?[] { "string", "a", new List<string>() { "a" }, typeof(string) };
            yield return new object?[] { "stringsAsListOfObject", new List<object>() { "a", "b", "c" }, new List<string>() { "a", "b", "c" }, typeof(string) };
            yield return new object?[] { "stringsAsArrayOfObject", new object[] { "a", "b", "c" }, new List<string>() { "a", "b", "c" }, typeof(string) };
        }
    }
}