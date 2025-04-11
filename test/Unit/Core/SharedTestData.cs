// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;

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
        
        static object?[] GetSampleValues(Type type)
        {
            Type t = Nullable.GetUnderlyingType(type) ?? type;

            List<object?> values = t switch
            {
                _ when t == typeof(string) => [ "Value" ],
                _ when t == typeof(int) => [ -1, 0, 1 ],
                _ when t == typeof(Guid) => [ Guid.Empty ],
                _ => throw new NotSupportedException($"No fuzz input for {type}")
            };

            if (Nullable.GetUnderlyingType(type) != null)
            {
                values.Add(null);
            }

            object?[] result = values.ToArray();
            return result;
        }
        
        public static IEnumerable<object?[]> ValuesForTypeTestData()
        {
            Type[] types = ConversionCapabilityHelper.WithNullableCounterparts([
                typeof(string),
                typeof(int)
            ]);

            foreach (Type type in types)
            {
                object?[] values = GetSampleValues(type);
                
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
        
        public static IEnumerable<object?[]> FromStringValueToTypeTestData()
        {
            yield return [typeof(bool), "true", true];
            yield return [typeof(bool), "True", true];
            // yield return [typeof(bool), "yes", true]; (does not work...)
            // yield return [typeof(bool), "1", true]; (does not work...)

            yield return [typeof(bool), "false", false];
            yield return [typeof(bool), "False", false];
            // yield return [typeof(bool), "no", false]; (does not work...)
            // yield return [typeof(bool), "0", false]; (does not work...)

            yield return [typeof(int), "-1", -1];
            yield return [typeof(int), "0", 0];
            yield return [typeof(int), " 3 ", 3];
            yield return [typeof(int), "42", 42];
            // yield return [typeof(int), "9.99", 10]; (does not work...)

            yield return [typeof(double), "-0.001", -0.001];
            yield return [typeof(double), "0", (double)0];
            yield return [typeof(double), "42", 42.0];
            yield return [typeof(double), "3.14", 3.14];
            yield return [typeof(double), " 9.99 ", 9.99];

            yield return [typeof(Guid), "550e8400-e29b-41d4-a716-446655440000", new Guid("550e8400-e29b-41d4-a716-446655440000")];
            yield return [typeof(Guid), "550E8400E29B41D4A716446655440000", new Guid("550e8400-e29b-41d4-a716-446655440000")]; // no hyphens (valid)
            yield return [typeof(Guid), "550E8400-E29B-41D4-A716-446655440000", new Guid("550e8400-e29b-41d4-a716-446655440000")]; // uppercase

#pragma warning disable RS0030
            yield return [typeof(DateTime), "2024-02-01T12:34:56Z", new DateTime(2024, 2, 1, 12, 34, 56, DateTimeKind.Utc)];
#pragma warning restore RS0030

            yield return [typeof(TimeSpan), "02:30:00", new TimeSpan(2, 30, 0)];
            yield return [typeof(TimeSpan), "0:00", TimeSpan.FromMinutes(0)];
            yield return [typeof(TimeSpan), "1:00", TimeSpan.FromHours(1)];

            yield return [typeof(Uri), "https://kaylumah.nl", new Uri("https://kaylumah.nl")];
            yield return [typeof(Uri), "https://www.kaylumah.nl", new Uri("https://www.kaylumah.nl")];
            yield return [typeof(Uri), "http://example.com", new Uri("http://example.com")];
            yield return [typeof(Uri), "http://www.example.com", new Uri("http://www.example.com")];

            yield return [typeof(CultureInfo), "nl-NL", new CultureInfo("nl-NL")];
            yield return [typeof(CultureInfo), "nl", new CultureInfo("nl")];
        }
        
        public static IEnumerable<object?[]> FromObjectValueToTypeTestData()
        {
            yield return [typeof(int), DayOfWeek.Sunday, 0];

            // Int conversions
            yield return [typeof(double), 42, 42.0];
            yield return [typeof(bool), 0, false];
            yield return [typeof(bool), 1, true];
            yield return [typeof(long), int.MinValue, (long)int.MinValue];
            yield return [typeof(long), int.MaxValue, (long)int.MaxValue];
            yield return [typeof(string), 123, "123"];

            // Double conversions
            yield return [typeof(int), 3.14, 3];
            yield return [typeof(int), 3.95, 4];
            yield return [typeof(float), 3.14, 3.14f];
            yield return [typeof(decimal), 3.14, (decimal)3.14];
            yield return [typeof(string), 3.14, "3.14"];

            // Bool conversions
            yield return [typeof(string), true, "True"];
            yield return [typeof(string), false, "False"];

            // DateTime conversions
#pragma warning disable RS0030
            yield return [typeof(string), new DateTime(2024, 2, 1, 12, 34, 56, DateTimeKind.Utc), "02/01/2024 12:34:56"];
#pragma warning restore RS0030

            // Guid conversions (not supported)
            Guid g = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
            yield return [typeof(string), g, "550e8400-e29b-41d4-a716-446655440000"];

            /*
               // Int to nullable int
               yield return [typeof(int?), 42, 42];

               // Bool to nullable bool
               yield return [typeof(bool?), true, true];
             */
        }
    }
}