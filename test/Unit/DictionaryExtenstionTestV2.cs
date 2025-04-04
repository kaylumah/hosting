// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Guid = System.Guid;
using Xunit;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class DictionaryExtenstionTestV2
    {
        static readonly Dictionary<Type, object?> _KnownTypes;
        
        static DictionaryExtenstionTestV2()
        {
            _KnownTypes = new Dictionary<Type, object?>
            {
                { typeof(string), null }
            };
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
            yield return [typeof(DateTime), "2024-02-01T12:34:56Z", new DateTime(2024, 2, 1, 12, 34, 56)];
            yield return [typeof(TimeSpan), "02:30:00", new TimeSpan(2, 30, 0)];
        }
        
        public static IEnumerable<object?[]> ParsedValueForObjectValueTestData()
        {
            yield return [typeof(double), 42, 42.0];
            yield return [typeof(bool), 0, 42.0];
            yield return [typeof(bool), 1, 42.0];
            // double 3.14 -> int
            // DateTime -> string
            
            
            /*
             * [InlineData(42, typeof(int), 42)]
               [InlineData(42, typeof(double), 42.0)]
               [InlineData(3.14, typeof(float), 3.14f)]
               // [InlineData(3.14, typeof(decimal), 3.14)]
               [InlineData(1, typeof(bool), true)]
               [InlineData(0, typeof(bool), false)]
               [InlineData(int.MaxValue, typeof(long), (long)int.MaxValue)]
               [InlineData(int.MinValue, typeof(long), (long)int.MinValue)]
             */
        }
        
        public static IEnumerable<object?[]> ParsedValueForStringThrowsTestData()
        {
            yield return [typeof(bool), "NotABool", typeof(FormatException)];
            yield return [typeof(int), "NotABool", typeof(FormatException)];
            yield return [typeof(Guid), "NotABool", typeof(FormatException)];
            yield return [typeof(DateTime), "NotABool", typeof(FormatException)];
            yield return [typeof(TimeSpan), "NotABool", typeof(FormatException)];
            // yield return [typeof(object), "NotABool", typeof(InvalidOperationException)];
        }
        
        public static IEnumerable<object?[]> ParsedValueForObjectThrowsTestData()
        {
            yield return [typeof(int), long.MaxValue, typeof(OverflowException)];
            yield return [typeof(int), new object(), typeof(OverflowException)];
            yield return [typeof(Uri), true, typeof(OverflowException)];
        }
        
        [Theory]
        [MemberData(nameof(DefaultValueForNullValueTestData))]
        public void Test_NullValue_ReturnsDefault(Type type, object? expected)
        {
            object? actual = ConvertValue(null, type);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [MemberData(nameof(DefaultValueForEmptyStringValueTestData))]
        public void Test_EmptyStringValue_ReturnsDefault(Type type, string input, object? expected)
        {
            object? actual = ConvertValue(input, type);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ParsedValueForStringValueTestData))]
        public void Test_StringValue_ReturnsParsedValue(Type type, string input, object? expected)
        {
            object? actual = ConvertValue(input, type);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ParsedValueForObjectValueTestData))]
        public void Test_ObjectValue_ReturnsParsedValue(Type type, object input, object expected)
        {
            object? actual = ConvertValue(input, type);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [MemberData(nameof(ParsedValueForStringThrowsTestData))]
        public void Test_StringValue_ThrowsException(Type type, string input, Type expectedExceptionType)
        {
            Exception ex = Assert.Throws(expectedExceptionType, () => ConvertValue(input, type));
            string exceptionMessage = ex.Message;
        }
        
        [Theory]
        [MemberData(nameof(ParsedValueForObjectThrowsTestData))]
        public void Test_ObjectValue_ThrowsException(Type type, object input, Type expectedExceptionType)
        {
            Exception ex = Assert.Throws(expectedExceptionType, () => ConvertValue(input, type));
            string exceptionMessage = ex.Message;
        }
        
        [Fact]
        public void Throws_For_TargetTypeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ConvertValue(null, null!));
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
            ArgumentNullException.ThrowIfNull(targetType);

            Type? nullableTargetType = Nullable.GetUnderlyingType(targetType);
            Type actualType = nullableTargetType ?? targetType;

            if (value is null)
            {
                object? result = DefaultForType(targetType);
                return result;
            }
            
            if (value is string strValue)
            {
                if (string.IsNullOrWhiteSpace(strValue))
                {
                    object? result = DefaultForType(targetType);
                    return result;
                }
                
                TypeConverter converter = TypeDescriptor.GetConverter(actualType);
                bool canConvert = converter.CanConvertFrom(typeof(string));

                if (canConvert)
                {
                    object? result = converter.ConvertFrom(strValue);
                    return result;
                }
                
                throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} as no TypeConverter exists.");
            }

            if (value is IConvertible convertible)
            {
                // throw new Exception("not string");

                try
                {
                    return Convert.ChangeType(convertible, actualType, CultureInfo.InvariantCulture);
                }
                catch (Exception ex) when (ex is OverflowException or InvalidCastException or FormatException)
                {
                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} via IConvertible.", ex);
                }
            }
            
            throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType}.");
        } 
    }
}