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
        static readonly MethodInfo _GetValuesMethod;
        static readonly ConcurrentDictionary<Type, MethodInfo> _GetValuesMethods;

        static DictionaryExtensionsTests()
        {
            Type type = typeof(DictionaryExtensions);

            _GetValuesMethod = type.GetMethod("GetValues")!;
            Debug.Assert(_GetValuesMethod != null);

            _GetValuesMethods = new();
            GetValuesMethod(typeof(string));
        }

        static MethodInfo GetValuesMethod(Type target)
        {
            MethodInfo methodInfo = _GetValuesMethods.GetOrAdd(target, (cacheKey) =>
            {
                MethodInfo result = _GetValuesMethod.MakeGenericMethod(cacheKey);
                return result;
            });

            return methodInfo;
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