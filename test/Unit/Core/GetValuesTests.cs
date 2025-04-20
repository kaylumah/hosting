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
    // TODO value conversion (single value)

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
            object[] arguments = [dictionary, key, true];
            object? result = getValueMethod?.Invoke(null, arguments);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<System.Collections.IList>(result);
            System.Collections.IList collection = (System.Collections.IList)result;
            Assert.Empty(collection);
        }

        [Fact]
        public void Test_GetValues_ExistingKeyWithNullValueReturnsDefaultValue()
        {
            string key = "some-key";
            Dictionary<string, object?> dictionary = new();
            dictionary[key] = null;
            Type targetType = typeof(string);
            MethodInfo getValueMethod = GetValuesMethod(targetType);
            object[] arguments = [dictionary, key, true];
            object? result = getValueMethod?.Invoke(null, arguments);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<System.Collections.IList>(result);
            System.Collections.IList collection = (System.Collections.IList)result;
            Assert.Empty(collection);
        }

        [Theory]
        [MemberData(nameof(SharedTestData.ValuesForTypeTestData), MemberType = typeof(SharedTestData))]
        public void Test_GetValues_ExactEnumerableMatchReturnsValue(Type type, object value)
        {
            AssertEnumerableOfT(type, value);

            Dictionary<string, object?> dictionary = new();
            dictionary["some-key"] = value;

            MethodInfo method = GetValuesMethod(type);
            object? actual = method.Invoke(null, [dictionary, "some-key", true]);

            Assert.Equal(value, actual);
        }

        public static IEnumerable<object?[]> GetEnumerableValueTestData2()
        {
            // Handles all other IEnumerables (non-exact)
            // TODO consider merging with [MemberData(nameof(SharedTestData.ValuesForTypeTestData), MemberType = typeof(SharedTestData))]

            yield return [typeof(string), new object?[] { "a", "b", "c" }, new string[] { "a", "b", "c" }];
            yield return [typeof(string), new List<object?>() { "a", "b", "c" }, new string[] { "a", "b", "c" }];

            yield return [typeof(int), new object?[] { "-1", "0", "1", null }, new int?[] { -1, 0, 1, 0 }];
            yield return [typeof(int?), new object?[] { "-1", "0", "1", null }, new int?[] { -1, 0, 1, null }];

            yield return [typeof(int?), new List<int> { 1, 2, 3 }, new int?[] { 1, 2, 3 }];

            System.Collections.ArrayList arrayList = new System.Collections.ArrayList { "1", "2", null };
            yield return [typeof(int?), arrayList, new int?[] { 1, 2, null }];
        }

        [Theory]
        [MemberData(nameof(GetEnumerableValueTestData2))]
        public void Test_GetValues_ConvertibleObjectList(Type type, object input, object expectedResult)
        {
            string key = "some-key";
            Dictionary<string, object?> dictionary = new();
            dictionary[key] = input;

            MethodInfo method = GetValuesMethod(type);
            object? actual = method.Invoke(null, [dictionary, key, true]);
            Assert.NotNull(actual);
            AssertEnumerableOfT(type, actual);

            Assert.IsAssignableFrom<System.Collections.IList>(actual);
            Assert.IsAssignableFrom<System.Collections.IList>(expectedResult);

            System.Collections.IList actualList = (System.Collections.IList)actual;
            System.Collections.IList expectedList = (System.Collections.IList)expectedResult;
            Assert.NotEmpty(actualList);
            Assert.Equal(expectedList.Count, actualList.Count);

            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.Equal(expectedList[i], actualList[i]);
            }
        }

        [Theory]
        [MemberData(nameof(SharedTestData.ValueForTypeTestData), MemberType = typeof(SharedTestData))]
        public void Test_GetValues_SingleConvertibleValueReturnsList(Type targetType, object? inputValue)
        {
            string key = "some-key";
            Dictionary<string, object?> dictionary = new();
            dictionary[key] = inputValue;

            MethodInfo method = GetValuesMethod(targetType);
            object? result = method.Invoke(null, [dictionary, key, true]);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<System.Collections.IList>(result);

            System.Collections.IList collection = (System.Collections.IList)result;
            object? item = collection[0];
            Assert.Equal(inputValue, item);
        }

        void AssertEnumerableOfT(Type type, object value)
        {
            Type genericIEnumerable = typeof(IEnumerable<>);
            Type expectedEnumerableType = genericIEnumerable.MakeGenericType(type);
            Type valueType = value.GetType();
            bool isEnumerable = expectedEnumerableType.IsAssignableFrom(valueType);
            Assert.True(isEnumerable, $"Expected {expectedEnumerableType} but was {valueType}");
        }
    }
}