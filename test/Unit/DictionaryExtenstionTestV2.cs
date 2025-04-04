// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Guid = System.Guid;
using System.Linq;
using System.Text;
using Xunit;

namespace Ssg.Extensions.Metadata.Abstractions
{
    static class ConversionCapabilityHelper
    {
        static readonly Type _StringType;
        
        static ConversionCapabilityHelper()
        {
            _StringType = typeof(string);
        }
        
        public static Type[] WithNullableCounterparts(IEnumerable<Type> types)
        {
            HashSet<Type> set = new HashSet<Type>();

            foreach (Type type in types)
            {
                set.Add(type);

                if (type.IsValueType && type != typeof(void))
                {
                    Type nullableType = typeof(Nullable<>).MakeGenericType(type);
                    set.Add(nullableType);
                }
            }

            Type[] result = set.ToArray();
            return result;
        }
        
        public static bool CanConvertFromString(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            Type actualType = Nullable.GetUnderlyingType(type) ?? type;
            TypeConverter converter = TypeDescriptor.GetConverter(actualType);
            bool canConvert = converter.CanConvertFrom(_StringType);
            return canConvert;
        }
        
        public static bool ImplementsIConvertible(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            Type actualType = Nullable.GetUnderlyingType(type) ?? type;
            bool result = typeof(IConvertible).IsAssignableFrom(actualType);
            return result;
        }
        
        static string GetFriendlyTypeName(Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type underlying)
            {
                return $"{underlying.Name}?";
            }

            return type.Name;
        }
        
        public static string GetTypeCompatibilityMatrix(params Type[] types)
        {
            StringBuilder sb = new StringBuilder();

            // Header
            sb.AppendLine(CultureInfo.InvariantCulture, $"{ "Type",-30} { "TypeConverter",-15} { "IConvertible",-15}");

            // Rows
            foreach (Type type in types)
            {
                string displayName = GetFriendlyTypeName(type);
                bool canConvert = CanConvertFromString(type);
                bool convertible = ImplementsIConvertible(type);

                sb.AppendLine(CultureInfo.InvariantCulture, $"{ displayName,-30} {(canConvert ? "✅" : "❌"),-15} {(convertible ? "✅" : "❌"),-15}");
            }

            string result = sb.ToString();
            return result;
        }
    }
    
    public class DictionaryExtenstionTestV2
    {
        static readonly Dictionary<Type, object?> _KnownTypes;
        
        static DictionaryExtenstionTestV2()
        {
            Type[] types = [ 
                typeof(string),
                typeof(int),
                typeof(bool)
            ];
            Type[] allTypes = ConversionCapabilityHelper.WithNullableCounterparts(types);
            
            #pragma warning disable
            string matrix = ConversionCapabilityHelper.GetTypeCompatibilityMatrix(allTypes);
            #pragma warning restore

            _KnownTypes = new Dictionary<Type, object?>();
            foreach (Type type in allTypes)
            {
                _KnownTypes[type] = DefaultForType(type);
            }
            
            /*
            {
                { typeof(string), null },
                // TODO
                /*
                { typeof(int), 0 },
                { typeof(int?), null },
                { typeof(DateTime), default(DateTime) },
                { typeof(DateTime?), null },
                { typeof(bool), false },
                { typeof(bool?), null }
            };
            */
              
                
            /*
             * [InlineData("https://kaylumah.nl", typeof(Uri), "https://kaylumah.nl")]
               [InlineData("1.2.3.4", typeof(Version), "1.2.3.4")]
               [InlineData("en-US", typeof(CultureInfo), "en-US")]
             */
        }

        public static IEnumerable<object?[]> DefaultValueForNullValueTestData()
        {
            foreach (KeyValuePair<Type, object?> x in _KnownTypes)
            {
                object?[] result = [x.Key, x.Value];
                yield return result;
            }
        }
        
        public static IEnumerable<object?[]> DefaultValueForEmptyStringValueTestData()
        {
            string[] values = [
                string.Empty,
                " ",
                "   "
            ];

            foreach (string value in values)
            {
                foreach (KeyValuePair<Type, object?> x in _KnownTypes)
                {
                    object?[] result = [x.Key, value, x.Value];
                    yield return result;
                } 
            }
        }

        public static IEnumerable<object?[]> ParsedValueForStringValueTestData()
        {
            #pragma warning disable
            // TODO 1 and 0?
            // TODO Yes and No?
            // [InlineData(typeof(bool), "true", true)]
            // [InlineData(typeof(bool), "false", false)]
            // [InlineData(typeof(int), "42", 42)]
            // [InlineData(typeof(Guid), "550e8400-e29b-41d4-a716-446655440000", new Guid("550e8400-e29b-41d4-a716-446655440000")]
            yield return [typeof(DateTime), "2024-02-01T12:34:56Z", new DateTime(2024, 2, 1, 12, 34, 56, DateTimeKind.Utc)];
            yield return [typeof(TimeSpan), "02:30:00", new TimeSpan(2, 30, 0)];
        }
        
        public static IEnumerable<object?[]> ParsedValueForObjectValueTestData()
        {
            // TODO int -> int?
            // TODO DateTime -> string
            
            // Int conversions
            yield return [typeof(double), 42, 42.0];
            yield return [typeof(bool), 0, false];
            yield return [typeof(bool), 1, true];
            yield return [typeof(long), int.MinValue, (long) int.MinValue];
            yield return [typeof(long), int.MaxValue, (long) int.MaxValue];
            
            // Double conversions
            yield return [typeof(int), 3.14, 3];
            yield return [typeof(int), 3.95, 4];
            yield return [typeof(float), 3.14, 3.14f];
            yield return [typeof(decimal), 3.14, (decimal) 3.14];
        }
        
        public static IEnumerable<object?[]> ParsedValueForStringThrowsTestData()
        {
            yield return [typeof(bool), "NotABool", typeof(FormatException)];
            yield return [typeof(int), "NotABool", typeof(ArgumentException)];
            yield return [typeof(Guid), "NotABool", typeof(FormatException)];
            yield return [typeof(DateTime), "NotABool", typeof(FormatException)];
            yield return [typeof(TimeSpan), "NotABool", typeof(FormatException)];
            // yield return [typeof(object), "NotABool", typeof(InvalidOperationException)];
        }
        
        public static IEnumerable<object?[]> ParsedValueForObjectThrowsTestData()
        {
            yield return [typeof(int), long.MaxValue, typeof(OverflowException)];
            yield return [typeof(Uri), true, typeof(InvalidCastException)];
            // yield return [typeof(int), new object(), typeof(OverflowException)];
        }
        
        [Fact]
        public void Test_ConvertValue_ThrowsForTargetTypeIsNull()
        {
            // Null suppression on purpose
            Assert.Throws<ArgumentNullException>(() => ConvertValue(null, null!));
        }
        
        [Theory]
        [MemberData(nameof(DefaultValueForNullValueTestData))]
        public void Test_ConvertValue_NullValueReturnsDefaultValue(Type targetType, object? expected)
        {
            object? actual = ConvertValue(null, targetType);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(DefaultValueForEmptyStringValueTestData))]
        public void Test_ConvertValue_EmptyStringValueReturnsDefaultValue(Type targetType, string input, object? expected)
        {
            object? actual = ConvertValue(input, targetType);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [MemberData(nameof(ParsedValueForStringValueTestData))]
        public void Test_ConvertValue_StringValueReturnsParsedValue(Type targetType, string input, object? expected)
        {
            object? actual = ConvertValue(input, targetType);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [MemberData(nameof(ParsedValueForStringThrowsTestData))]
        public void Test_ConvertValue_StringValueThrowsExceptionOnConversionFailure(Type type, string input, Type expectedExceptionType)
        {
            InvalidOperationException outerException = Assert.Throws<InvalidOperationException>(() => ConvertValue(input, type));
            
            Assert.NotNull(outerException);
            Assert.NotNull(outerException.InnerException);

            Exception inner = outerException.InnerException;
            Assert.IsType(expectedExceptionType, inner);
        }
        
        [Theory]
        [MemberData(nameof(ParsedValueForObjectValueTestData))]
        public void Test_ConvertValue_ObjectValueReturnsParsedValue(Type type, object input, object expected)
        {
            object? actual = ConvertValue(input, type);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [MemberData(nameof(ParsedValueForObjectThrowsTestData))]
        public void Test_ConvertValue_ObjectValueThrowsExceptionOnConversionFailure(Type type, object input, Type expectedExceptionType)
        {
            InvalidOperationException outerException = Assert.Throws<InvalidOperationException>(() => ConvertValue(input, type));
            
            Assert.NotNull(outerException);
            Assert.NotNull(outerException.InnerException);

            Exception inner = outerException.InnerException;
            Assert.IsType(expectedExceptionType, inner);
        }
        
        public static object? DefaultForType(Type targetType)
        {
            Type? nullableTargetType = Nullable.GetUnderlyingType(targetType);
            bool isValueType = targetType.IsValueType;
            bool isNonNullableType = nullableTargetType is null;
            bool returnDefault = isValueType && isNonNullableType;
            
            if (returnDefault)
            {
                object? result = Activator.CreateInstance(targetType);
                return result;
            }

            return null;
        }
        
        public static object? ConvertValue(object? value, Type targetType)
        {
            // Original method
            // object? result = value.ConvertValue(targetType);
            // return result;
            
            // IConvertible
            // bool, byte, char, short, int, long, float, double, decimal, string, DateTime, Enum

            // TypeConverter
            // string, bool, int, double, DateTime, TimeSpan, Guid, Uri, Version ,CultureInfo, Enum, Nullable<T>

            ArgumentNullException.ThrowIfNull(targetType);

            Type? nullableTargetType = Nullable.GetUnderlyingType(targetType);
            Type actualType = nullableTargetType ?? targetType;

            // 1. Null case
            if (value is null)
            {
                object? result = DefaultForType(targetType);
                return result;
            }

            // 2. Already the correct type
            if (value.GetType() == actualType)
            {
                // return value;
            }

            // 3. String input
            if (value is string strValue)
            {
                if (string.IsNullOrWhiteSpace(strValue))
                {
                    object? result = DefaultForType(targetType);
                    return result;
                }

                /*
                if (actualType == typeof(DateTime))
                {
                    if (DateTime.TryParse(strValue, CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                            out DateTime parsedDateTime))
                    {
                        return parsedDateTime;
                    }

                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} due to incorrect format.");
                }
                */

                TypeConverter converter = TypeDescriptor.GetConverter(actualType);
                bool canConvert = converter.CanConvertFrom(typeof(string));

                if (canConvert)
                {
                    try
                    {
                        // converter.ConvertFromInvariantString?
                        object? result = converter.ConvertFrom(strValue);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} via TypeConverter.", ex);
                    }
                }

                throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} as no TypeConverter exists.");
            }

            // 4. IConvertible fallback
            if (value is IConvertible convertible)
            {
                try
                {
                    return Convert.ChangeType(convertible, actualType, CultureInfo.InvariantCulture);
                }
                catch (Exception ex) when (ex is OverflowException or InvalidCastException or FormatException)
                {
                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} via IConvertible.", ex);
                }
            }

            // Conversion failed
            throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType}.");
        } 
    }
}