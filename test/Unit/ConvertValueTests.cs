// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Guid = System.Guid;
using Xunit;

namespace Test.Unit
{
    public class ConvertValueTests
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

        public static IEnumerable<object?[]> StringValueTestData()
        {
            yield return [typeof(bool), "true", true];
            yield return [typeof(bool), "True", true];
            // yield return [typeof(bool), "yes", true]; (does not work...)
            // yield return [typeof(bool), "1", true]; (does not work...)

            yield return [typeof(bool), "false", false];
            yield return [typeof(bool), "False", false];
            // yield return [typeof(bool), "no", false]; (does not work...)
            // yield return [typeof(bool), "0", false]; (does not work...)

            yield return [typeof(int), "42", 42];

            yield return [typeof(double), "42.0", 42.0];

            yield return [typeof(Guid), "550e8400-e29b-41d4-a716-446655440000", new Guid("550e8400-e29b-41d4-a716-446655440000")];

#pragma warning disable RS0030
            yield return [typeof(DateTime), "2024-02-01T12:34:56Z", new DateTime(2024, 2, 1, 12, 34, 56, DateTimeKind.Utc)];
#pragma warning restore RS0030

            yield return [typeof(TimeSpan), "02:30:00", new TimeSpan(2, 30, 0)];

            yield return [typeof(Uri), "https://kaylumah.nl", new Uri("https://kaylumah.nl")];

            yield return [typeof(CultureInfo), "nl-NL", new CultureInfo("nl-NL")];

            // int
            // "0", "-1", " 42 " (trailing/leading spaces)

            // double
            // "3.14", "-0.001", "42" (no decimal)

            // guid
            // Uppercase string, compact format (no hyphens)

            // CultureInfo
            // nl

            /*
             * // int
               yield return [typeof(int), "0", 0];
               yield return [typeof(int), "-1", -1];
               yield return [typeof(int), " 42 ", 42];
               
               // double
               yield return [typeof(double), "3.14", 3.14];
               yield return [typeof(double), "42", 42.0];
               
               // Guid
               yield return [typeof(Guid), "550E8400E29B41D4A716446655440000", new Guid("550e8400-e29b-41d4-a716-446655440000")]; // no hyphens (valid)
               yield return [typeof(Guid), "550E8400-E29B-41D4-A716-446655440000", new Guid("550e8400-e29b-41d4-a716-446655440000")]; // uppercase
               
               // TimeSpan
               yield return [typeof(TimeSpan), "0:00", TimeSpan.FromMinutes(0)];
               yield return [typeof(TimeSpan), "1:00", TimeSpan.FromHours(1)];
               
               // DateTime
               yield return [typeof(DateTime), "2024-02-01T12:34:56", new DateTime(2024, 2, 1, 12, 34, 56, DateTimeKind.Unspecified)];
               yield return [typeof(DateTime), "2024-02-01T12:34:56+00:00", DateTime.Parse("2024-02-01T12:34:56+00:00", null, DateTimeStyles.RoundtripKind)];
               
               // CultureInfo
               yield return [typeof(CultureInfo), "en", new CultureInfo("en")];
               
               // Uri
               yield return [typeof(Uri), "http://example.com", new Uri("http://example.com")];
               
               // Version
               yield return [typeof(Version), "1.2.3.4", new Version("1.2.3.4")];
             */
        }

        public static IEnumerable<object?[]> ObjectValueTestData()
        {
            // TODO int -> int?
            // TODO DateTime -> string

            // Int conversions
            yield return [typeof(double), 42, 42.0];
            yield return [typeof(bool), 0, false];
            yield return [typeof(bool), 1, true];
            yield return [typeof(long), int.MinValue, (long)int.MinValue];
            yield return [typeof(long), int.MaxValue, (long)int.MaxValue];

            // Double conversions
            yield return [typeof(int), 3.14, 3];
            yield return [typeof(int), 3.95, 4];
            yield return [typeof(float), 3.14, 3.14f];
            yield return [typeof(decimal), 3.14, (decimal)3.14];

            /*
             * // bool → string
               yield return [typeof(string), true, "True"];
               yield return [typeof(string), false, "False"];
               
               // int → string
               yield return [typeof(string), 123, "123"];
               
               // double → string
               yield return [typeof(string), 3.14, "3.14"];
               
               // DateTime → string
               yield return [typeof(string), new DateTime(2024, 2, 1, 12, 34, 56, DateTimeKind.Utc), "2024-02-01T12:34:56Z"];
               
               // Guid → string
               Guid g = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
               yield return [typeof(string), g, g.ToString()];
               
               // Int to nullable int
               yield return [typeof(int?), 42, 42];
               
               // Bool to nullable bool
               yield return [typeof(bool?), true, true];
             */
        }

        public static IEnumerable<object?[]> StringValueThrowsTestData()
        {
            yield return [typeof(bool), "abc", typeof(FormatException)];
            yield return [typeof(int), "abc", typeof(ArgumentException)];
            yield return [typeof(double), "abc", typeof(ArgumentException)];
            yield return [typeof(Guid), "abc", typeof(FormatException)];
#pragma warning disable RS0030
            yield return [typeof(DateTime), "abc", typeof(FormatException)];
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
        [MemberData(nameof(DefaultValueForNullValueTestData))]
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
        [MemberData(nameof(StringValueTestData))]
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
        [MemberData(nameof(ObjectValueTestData))]
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

        static string GetSampleValue(Type type)
        {
            // todo faker?
            Type t = Nullable.GetUnderlyingType(type) ?? type;

            return t switch
            {
                _ when t == typeof(string) => "",
                _ => throw new NotSupportedException($"No fuzz input for {type}")
            };
        }

        static object? ConvertValue(object? value, Type targetType)
        {
            object? result = value.ConvertValue(targetType);
            return result;
        }
    }
}