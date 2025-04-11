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
        static object GetSampleValue(Type type)
        {
            // todo faker?
            Type t = Nullable.GetUnderlyingType(type) ?? type;

            return t switch
            {
                _ when t == typeof(string) => "Value",
                _ when t == typeof(int) => 42,
                _ => throw new NotSupportedException($"No fuzz input for {type}")
            };
        }

        public static IEnumerable<object?[]> ValueForTypeTestData()
        {
            Type[] types = ConversionCapabilityHelper.WithNullableCounterparts([
                typeof(string),
                typeof(int)
            ]);

            foreach (Type type in types)
            {
                object value = GetSampleValue(type);
                object?[] result = [type, value];
                yield return result;
            }
        }
        
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
        [MemberData(nameof(SharedTestData.DefaultValueForNullValueTestData), MemberType = typeof(SharedTestData))]
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