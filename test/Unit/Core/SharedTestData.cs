// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

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
        
        static object[] GetSampleValues(Type type)
        {
            // todo faker?
            Type t = Nullable.GetUnderlyingType(type) ?? type;

            return t switch
            {
                _ when t == typeof(string) => [ "Value" ],
                _ when t == typeof(int) => [ -1, 0 , 1 ],
                _ when t == typeof(Guid) => [ Guid.Empty ],
                _ => throw new NotSupportedException($"No fuzz input for {type}")
            };
        }
        
        public static IEnumerable<object?[]> ValuesForTypeTestData()
        {
            Type[] types = ConversionCapabilityHelper.WithNullableCounterparts([
                typeof(string),
                typeof(int)
            ]);

            foreach (Type type in types)
            {
                object[] values = GetSampleValues(type);
                
                /*
                Array typedArray = Array.CreateInstance(type, values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    typedArray.SetValue(values[i], i);
                }

                object?[] result = [type, typedArray];
                yield return result;
                */
                
                Type listType = typeof(List<>).MakeGenericType(type);
                System.Collections.IList typedList = (System.Collections.IList)Activator.CreateInstance(listType)!;
                foreach (object? value in values)
                {
                    typedList.Add(value);
                }
    
                yield return [ type, typedList ];

                // object?[] result = [type, values];
                // yield return result;
            }
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
        
        public static IEnumerable<object?[]> DefaultValueForTypeTestData()
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
}