// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Globalization;

namespace System.Collections.Generic
{
    public static class ObjectExtensions
    {
        /*
           if (bool.TryParse(strValue, out bool boolResult))
           {
               return boolResult;
           }
           
           if (int.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out int intResult))
           {
               return intResult;
           }

           if (Guid.TryParse(strValue, out Guid guidResult))
           {
               return guidResult;
           }
           
           if (DateTime.TryParse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dateTimeResult))
           {
               return dateTimeResult;
           }
           
           if (TimeSpan.TryParse(strValue, CultureInfo.InvariantCulture, out TimeSpan timeSpanResult))
           {
               return timeSpanResult;
           }
           
           // if (targetType == typeof(byte) && byte.TryParse(strValue, out byte byteResult)) return byteResult;
           // if (targetType == typeof(sbyte) && sbyte.TryParse(strValue, out sbyte sbyteResult)) return sbyteResult;
           // if (targetType == typeof(short) && short.TryParse(strValue, out short shortResult)) return shortResult;
           // if (targetType == typeof(ushort) && ushort.TryParse(strValue, out ushort ushortResult)) return ushortResult;
           // if (targetType == typeof(long) && long.TryParse(strValue, out long longResult)) return longResult;
           // if (targetType == typeof(ulong) && ulong.TryParse(strValue, out ulong ulongResult)) return ulongResult;
           // if (targetType == typeof(float) && float.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatResult)) return floatResult;
           // if (targetType == typeof(double) && double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleResult)) return doubleResult;
           // if (targetType == typeof(decimal) && decimal.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalResult)) return decimalResult;
           
         */

        static readonly Dictionary<(Type from, Type to), Func<object, object?>> _Converters;

        static ObjectExtensions()
        {
            _Converters = new();
#pragma warning disable RS0030
            _Converters[(typeof(string), typeof(DateTime))] = (value) =>
            {
                if (value is not string strValue || string.IsNullOrWhiteSpace(strValue))
                {
                    object? result = DefaultForType(typeof(DateTime));
                    return result;
                }

                if (DateTime.TryParse(strValue, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out DateTime parsedDateTime))
                {
                    return parsedDateTime;
                }

                throw new InvalidOperationException($"Cannot convert value '{value}' to {typeof(DateTime)} due to incorrect format.");
            };
#pragma warning restore RS0030
        }

        public static object? DefaultForType(this Type targetType)
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

        public static bool TryGetConverter(
            Type targetType,
            Type inputType,
            [Diagnostics.CodeAnalysis.NotNullWhen(true)] out TypeConverter? converter)
        {
            TypeConverter candidate = TypeDescriptor.GetConverter(targetType);

            if (candidate.CanConvertFrom(inputType))
            {
                converter = candidate;
                return true;
            }

            converter = null;
            return false;
        }

        public static object? ConvertValue(this object? value, Type targetType)
        {
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

            Type sourceType = value.GetType();

            // 2. Already the correct type
            if (sourceType == actualType)
            {
                return value;
            }

            if (_Converters.TryGetValue((sourceType, targetType), out Func<object, object?>? converter))
            {
                try
                {
                    object? result = converter(value);
                    return result;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} via TypeConverter.", ex);
                    // throw new InvalidOperationException($"Custom conversion from {value.GetType()} to {actualType} failed.", ex);
                }
            }

            // 3. String input
            if (value is string strValue)
            {
                if (string.IsNullOrWhiteSpace(strValue))
                {
                    object? result = DefaultForType(targetType);
                    return result;
                }

                if (TryGetConverter(actualType, typeof(string), out TypeConverter? stringConverter))
                {
                    try
                    {
                        object? result = stringConverter.ConvertFromInvariantString(strValue);
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
                    object? result = Convert.ChangeType(convertible, actualType, CultureInfo.InvariantCulture);
                    return result;
                }
                catch (Exception ex) when (ex is OverflowException or InvalidCastException or FormatException)
                {
                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} via IConvertible.", ex);
                }
            }

            // 5. Handle to string?
            if (targetType == typeof(string))
            {
                string? result = value?.ToString();
                return result;
            }

            /*
            // ?. Try using Converter
            if (TryGetConverter(actualType, value.GetType(), out TypeConverter? converter))
            {
                
            }
            */

            // Conversion failed
            throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType}.");
        }
    }
}