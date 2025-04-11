// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Guid = System.Guid;
using Xunit;

namespace Test.Unit.Core
{
    public class ConvertValueTests
    {
        public static IEnumerable<object?[]> DefaultValueForEmptyStringValueTestData()
        {
            string[] values =
            [
                string.Empty,
                " ",
                "   "
            ];

            Type[] types = ConversionCapabilityHelper.WithNullableCounterparts(
                [
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
                foreach (string value in values)
                {
                    object?[] result = [type, value, defaultValue];
                    yield return result;
                }
            }

            // String would match on the short-circuit
            Type stringType = typeof(string);
            foreach (string value in values)
            {
                object?[] result = [stringType, value, value];
                yield return result;
            }
        }
        
        public static IEnumerable<object?[]> StringValueThrowsTestData()
        {
            yield return [typeof(bool), "abc", typeof(FormatException)];
            yield return [typeof(int), "abc", typeof(ArgumentException)];
            yield return [typeof(double), "abc", typeof(ArgumentException)];
            yield return [typeof(Guid), "abc", typeof(FormatException)];
#pragma warning disable RS0030
            yield return [typeof(DateTime), "abc", typeof(InvalidOperationException)];
#pragma warning restore RS0030
            yield return [typeof(TimeSpan), "abc", typeof(FormatException)];
        }

        public static IEnumerable<object?[]> ObjectValueThrowsTestData()
        {
            yield return [typeof(int), long.MaxValue, typeof(OverflowException)];
            yield return [typeof(Uri), true, typeof(InvalidCastException)];
        }

        [Fact]
        public void Test_ConvertValue_ThrowsForTargetTypeIsNull()
        {
            // Null suppression on purpose
            Assert.Throws<ArgumentNullException>(() => ObjectExtensions.ConvertValue(null, null!));
        }

        [Theory]
        [MemberData(nameof(SharedTestData.DefaultValueForTypeTestData), MemberType = typeof(SharedTestData))]
        public void Test_ConvertValue_NullValueReturnsDefaultValue(Type targetType, object? expected)
        {
            object? actual = ObjectExtensions.ConvertValue(null, targetType);
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
        [MemberData(nameof(SharedTestData.FromStringValueToTypeTestData), MemberType = typeof(SharedTestData))]
        public void Test_ConvertValue_StringValueReturnsParsedValue(Type targetType, string input, object? expected)
        {
            object? actual = ConvertValue(input, targetType);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(StringValueThrowsTestData))]
        public void Test_ConvertValue_StringValueThrowsExceptionOnConversionFailure(Type type, string input, Type expectedExceptionType)
        {
            InvalidOperationException outerException = Assert.Throws<InvalidOperationException>(() => ConvertValue(input, type));

            Assert.NotNull(outerException);

            if (outerException.InnerException == null)
            {
                string expectedErrorMessage = $"Cannot convert value '{input}' to {type.FullName} due to incorrect format.";
                Assert.Equal(expectedErrorMessage, outerException.Message);
            }
            else
            {
                Exception inner = outerException.InnerException;
                Assert.IsType(expectedExceptionType, inner);
                string expectedErrorMessage = $"Cannot convert value '{input}' to {type.FullName} via TypeConverter.";
                Assert.Equal(expectedErrorMessage, outerException.Message);
            }
        }

        [Fact]
        public void Test_ConvertValue_StringValueThrowsExceptionOnMissingTypeConverter()
        {
            string input = "NotABool";
            Type type = typeof(object);
            InvalidOperationException outerException = Assert.Throws<InvalidOperationException>(() => ConvertValue(input, type));
            Assert.Null(outerException.InnerException);
            string expectedErrorMessage = $"Cannot convert value '{input}' to {type.FullName} as no TypeConverter exists.";
            Assert.Equal(expectedErrorMessage, outerException.Message);
        }

        [Theory]
        [MemberData(nameof(SharedTestData.FromObjectValueToTypeTestData), MemberType = typeof(SharedTestData))]
        public void Test_ConvertValue_ObjectValueReturnsParsedValue(Type type, object input, object expected)
        {
            object? actual = ConvertValue(input, type);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ObjectValueThrowsTestData))]
        public void Test_ConvertValue_ObjectValueThrowsExceptionOnConversionFailure(Type type, object input, Type expectedExceptionType)
        {
            InvalidOperationException outerException = Assert.Throws<InvalidOperationException>(() => ConvertValue(input, type));

            Assert.NotNull(outerException);
            Assert.NotNull(outerException.InnerException);

            Exception inner = outerException.InnerException;
            Assert.IsType(expectedExceptionType, inner);

            string expectedErrorMessage = $"Cannot convert value '{input}' to {type.FullName} via IConvertible.";
            Assert.Equal(expectedErrorMessage, outerException.Message);
        }

        [Fact]
        public void Test_ConvertValue_UnsupportedType_ThrowsFinalFallback()
        {
            object value = new object(); // not convertible
            Type targetType = typeof(Uri); // non-IConvertible, won't work with Convert.ChangeType either

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => ConvertValue(value, targetType));

            Assert.Null(ex.InnerException); // because you're throwing directly, not wrapping
            Assert.Equal($"Cannot convert value '{value}' to {targetType}.", ex.Message);
        }

        static object? ConvertValue(object? value, Type targetType)
        {
            object? result = value.ConvertValue(targetType);
            return result;
        }
    }
}