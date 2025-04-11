// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace Test.Unit.Core
{
    public class GetValuesTests
    {
        static readonly MethodInfo _GetValuesMethod;
        static readonly ConcurrentDictionary<Type, MethodInfo> _GetValuesMethods;

        static GetValuesTests()
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
        
        [Fact]
        public void Test_GetValues_ArgumentNullExceptionForNullDictionary()
        {
            Dictionary<string, object?>? target = null;
            Assert.Throws<ArgumentNullException>(() => target!.GetValues<string>("some-key"));
        }

        [Fact]
        public void Test_GetValues_ArgumentNullExceptionForNullKey()
        {
            Dictionary<string, object?> target = new();
            Assert.Throws<ArgumentNullException>(() => target.GetValues<string>(null!));
        }
        
        [Fact]
        public void Test_GetValues_NonExistingKeyReturnsDefaultValue()
        {
            string key = "some-key";
            Dictionary<string, object?> dictionary = new();
            Type targetType = typeof(string);
            MethodInfo getValueMethod = GetValuesMethod(targetType);
            object[] arguments = [ dictionary, key, true ];
            object? actual = getValueMethod?.Invoke(null, arguments);
            Assert.Null(actual);
        }

        [Fact]
        public void Test_GetValue_ExistingKeyWithNullValueReturnsDefaultValue()
        {
            string key = "some-key";
            Dictionary<string, object?> dictionary = new();
            dictionary[key] = null;
            Type targetType = typeof(string);
            MethodInfo getValueMethod = GetValuesMethod(targetType);
            object[] arguments = [ dictionary, key, true ];
            object? actual = getValueMethod?.Invoke(null, arguments);
            Assert.Null(actual);
        }
        
        [Fact]
        public void Test_GetValues_PrimitiveOfTargetTypeReturnsList()
        {
            Dictionary<string, object?> dictionary = new()
            {
                { "NotAList", "42" }
            };

            MethodInfo? method = GetValuesMethod(typeof(string));
            object[] arguments = new object[] { dictionary, "NotAList", true };
            object? result = method?.Invoke(null, arguments);
            
            string[] expected = new[] { "42" };
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void Test_GetValues_PrimitiveOfOtherTypeThrows()
        {
            string key = "NotAList";
            Dictionary<string, object?> dictionary = new()
            {
                { "NotAList", 42 }
            };

            MethodInfo? method = GetValuesMethod(typeof(string));
            object[] arguments = new object[] { dictionary, "NotAList", true };
            TargetInvocationException targetInvocationException = Assert.Throws<TargetInvocationException>(() => method?.Invoke(null, arguments));
            Assert.NotNull(targetInvocationException.InnerException);
            InvalidOperationException invalidOperationException = Assert.IsType<InvalidOperationException>(targetInvocationException.InnerException);

            string expectedErrorMessage = $"Cannot convert value of key '{key}' from System.Int32 to IEnumerable<System.String>.";
            Assert.Equal(expectedErrorMessage, invalidOperationException.Message);
        }

        public static IEnumerable<object?[]> GetEnumerableValueTestData()
        {
            yield return [ typeof(string), Array.Empty<string>() ];
            yield return [ typeof(string), new string[] { "a", "b", "c" } ];
            yield return [ typeof(string), new List<string> { "a", "b", "c" } ];
        }

        [Theory]
        [MemberData(nameof(GetEnumerableValueTestData))]
        public void Test_GetValues_ExactEnumerableMatchReturnsValue(Type type, object value)
        {
            Type genericIEnumerable = typeof(IEnumerable<>);
            Type expectedEnumerableType = genericIEnumerable.MakeGenericType(type);
            Type valueType = value.GetType();
            bool isEnumerable = expectedEnumerableType.IsAssignableFrom(valueType);
            Assert.True(isEnumerable);

            Dictionary<string, object?> dictionary = new();
            dictionary["some-key"] = value;
            
            MethodInfo method = GetValuesMethod(type);
            object? actual = method.Invoke(null, [ dictionary, "some-key", true ]);

            Assert.Equal(value, actual);
        }
        
        public static IEnumerable<object?[]> GetEnumerableValueTestData2()
        {
            // Type of INT throws
            yield return [ typeof(string), new object[] { "a", "b", "c" } ];
            yield return [ typeof(int), new object[] { "-1", "0", "1" } ];
        }
        
        [Theory]
        [MemberData(nameof(GetEnumerableValueTestData2))]
        public void Test_GetValues_ConvertibleObjectList(Type type, object input)
        {
            Dictionary<string, object?> dictionary = new();
            dictionary["some-key"] = input;
            
            MethodInfo method = GetValuesMethod(type);
            object? actual = method.Invoke(null, [ dictionary, "some-key", true ]);
            
            /*
            bool isListOfExpectedType = actual != null &&
                actual.GetType().IsGenericType &&
                                        actual.GetType().GetGenericTypeDefinition() == typeof(List<>) &&
                                        actual.GetType().GetGenericArguments()[0] == type;
                                        */
            
            /*
             * Type genericIEnumerable = typeof(IEnumerable<>);
               Type expectedEnumerableType = genericIEnumerable.MakeGenericType(targetType);
             */

        }
        
        /*
        [Fact]
        public void Test_GetValues_ObjectListWithInvalidEntriesSkipsThem()
        {
            var inputList = new List<object> { "1", "not-a-number", "3" };
            var dict = new Dictionary<string, object?> { ["some-key"] = inputList };

            var method = GetValuesMethod(typeof(int));
            object? result = method.Invoke(null, new object[] { dict, "some-key", true });

            var expected = new[] { 1, 3 }; // assuming failed conversions are skipped
            Assert.Equal(expected, result);
        }
        */
        
        /*
        [Fact]
        public void Test_GetValues_NullableBoolList_To_NonNullable()
        {
            IEnumerable<bool?> nullableBools = new[] { true, null, false };
            var dict = new Dictionary<string, object?> { ["some-key"] = nullableBools };

            var method = GetValuesMethod(typeof(bool));
            object? result = method.Invoke(null, new object[] { dict, "some-key", true });

            var expected = new[] { true, false };
            Assert.Equal(expected, result); // assuming nulls are skipped
        }
        */
        
        /*
        [Fact]
        public void Test_GetValues_NonNullableBoolList_To_Nullable()
        {
            IEnumerable<bool> nonNullableBools = new[] { true, false };
            var dict = new Dictionary<string, object?> { ["some-key"] = nonNullableBools };

            var method = GetValuesMethod(typeof(bool?));
            object? result = method.Invoke(null, new object[] { dict, "some-key", true });

            var expected = new bool?[] { true, false };
            Assert.Equal(expected, result);
        }
        */
    }
}