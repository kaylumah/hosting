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
    // TODO value conversion
    
    public class GetValueTests
    {
        static readonly ConcurrentDictionary<Type, MethodInfo> _GetValueMethods;
        static readonly MethodInfo _GetValueMethod;

        static GetValueTests()
        {
            Type type = typeof(DictionaryExtensions);
            
            _GetValueMethod = type.GetMethod("GetValue")!;
            Debug.Assert(_GetValueMethod != null);
            
            _GetValueMethods = new();
            GetValueMethod(typeof(string));
        }
        
        static MethodInfo GetValueMethod(Type target)
        {
            MethodInfo methodInfo = _GetValueMethods.GetOrAdd(target, (cacheKey) =>
            {
                MethodInfo result = _GetValueMethod.MakeGenericMethod(cacheKey);
                return result;
            });

            return methodInfo;
        }
        
        [Fact]
        public void Test_GetValue_ArgumentNullExceptionForNullDictionary()
        {
            Dictionary<string, object?>? target = null;
            Assert.Throws<ArgumentNullException>(() => target!.GetValue<string>("some-key"));
        }

        [Fact]
        public void Test_GetValue_ArgumentNullExceptionForNullKey()
        {
            Dictionary<string, object?> target = new();
            Assert.Throws<ArgumentNullException>(() => target.GetValue<string>(null!));
        }
        
        [Theory]
        [MemberData(nameof(SharedTestData.DefaultValueForTypeTestData), MemberType = typeof(SharedTestData))]
        public void Test_GetValue_NonExistingKeyReturnsDefaultValue(Type targetType, object? expected)
        {
            string key = "some-key";
            Dictionary<string, object?> dictionary = new();
            MethodInfo getValueMethod = GetValueMethod(targetType);
            object[] arguments = [ dictionary, key, true ];
            object? actual = getValueMethod?.Invoke(null, arguments);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [MemberData(nameof(SharedTestData.DefaultValueForTypeTestData), MemberType = typeof(SharedTestData))]
        public void Test_GetValue_ExistingKeyWithNullValueReturnsDefaultValue(Type targetType, object? expected)
        {
            string key = "some-key";
            Dictionary<string, object?> dictionary = new();
            dictionary[key] = null;
            MethodInfo getValueMethod = GetValueMethod(targetType);
            object[] arguments = [ dictionary, key, true ];
            object? actual = getValueMethod?.Invoke(null, arguments);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SharedTestData.ValueForTypeTestData), MemberType = typeof(SharedTestData))]
        public void Test_GetValue_ExactMatchReturnsValue(Type targetType, object? expected)
        {
            string key = "some-key";
            Dictionary<string, object?> dictionary = new();
            dictionary[key] = expected;
            MethodInfo getValueMethod = GetValueMethod(targetType);
            object[] arguments = [ dictionary, key, true ];
            object? actual = getValueMethod?.Invoke(null, arguments);
            Assert.Equal(expected, actual);
        }
    }
}