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
    public static class SharedTestData
    {
        public static IEnumerable<object?[]> DefaultValueForNullValueTestData()
        {
            Type[] types = ConversionCapabilityHelper.WithNullableCounterparts(
                [
                    typeof(string),
                    typeof(int),
                    typeof(bool),
                    typeof(Guid),
#pragma warning disable RS0030
                    typeof(DateTime),
#pragma warning restore RS0030
                    typeof(Uri)

                ]
            );
            foreach (Type type in types)
            {
                object? defaultValue = type.DefaultForType();
                object?[] result = [type, defaultValue];
                yield return result;
            }
        }
    }
    
    public class GetValueTests
    {
        // Exact Type Match (string -> string)
        // caseInsensitive: true
        // caseInsensitive: false
        // from string, from int, etc
        // Missing Key
        // NullValue for Key
        // Datetime -> Guid?
        // notabool -> bool bool?

        
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
        
        [Theory]
        [MemberData(nameof(SharedTestData.DefaultValueForNullValueTestData), MemberType = typeof(SharedTestData))]
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
    }
}