// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public static class Helper
    {
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
                
                #pragma warning disable RS0030
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
                #pragma warning restore RS0030
                
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
                    object? result = Convert.ChangeType(convertible, actualType, CultureInfo.InvariantCulture);
                    return result;
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
    
    
    public static class DictionaryExtensions
    {
        public static T? GetValue<T>(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            string lookupKey = dictionary.LookupKey(key, caseInsensitive);

            if (!dictionary.TryGetValue(lookupKey, out object? value))
            {
                return default;
            }

            if (value is null)
            {
                return default;
            }

            if (value is T exactMatch)
            {
                return exactMatch;
            }

            T? result = (T?)ConvertValue(value, typeof(T));
            return result;
        }

        public static IEnumerable<T>? GetValues<T>(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            string lookupKey = dictionary.LookupKey(key, caseInsensitive);

            if (!dictionary.TryGetValue(lookupKey, out object? value))
            {
                return default;
            }

            if (value is null)
            {
                return default;
            }

            if (value is IEnumerable<T> exactMatch)
            {
                return exactMatch;
            }

            if (value is T singleValue)
            {
                List<T> result = new List<T>() { singleValue };
                return result;
            }

            if (value is IEnumerable<object> objectList)
            {
                List<T> result = new List<T>();
                foreach (object original in objectList)
                {
                    T? converted = (T?)ConvertValue(original, typeof(T));
                    if (converted != null)
                    {
                        result.Add(converted);
                    }
                }

                return result;
            }

            throw new InvalidOperationException($"Cannot convert value of key '{key}' from {value?.GetType()} to IEnumerable<{typeof(T)}>.");
        }

        public static void SetValue(this Dictionary<string, object?> dictionary, string key, object? value, bool caseInsensitive = true)
        {
            string lookupKey = dictionary.LookupKey(key, caseInsensitive);
            dictionary[lookupKey] = value;
        }

        static string LookupKey(this Dictionary<string, object?> dictionary, string key, bool caseInsensitive = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);
            Debug.Assert(caseInsensitive);
            string lookupKey = caseInsensitive
                ? dictionary.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase)) ?? key
                : key;
            return lookupKey;
        }

        public static object? ConvertValue(this object? value, Type targetType)
        {
            if (value is null)
            {
                return targetType.IsValueType ? default : null;
            }

            if (value is string strValue)
            {
                // Handle built-in types with TryParse support
                if (targetType == typeof(bool))
                {
                    if (bool.TryParse(strValue, out bool boolResult))
                    {
                        return boolResult;
                    }

                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} due to incorrect format.");
                }

                if (targetType == typeof(int))
                {
                    if (int.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out int intResult))
                    {
                        return intResult;
                    }

                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} due to incorrect format.");
                }

                if (targetType == typeof(Guid))
                {
                    if (Guid.TryParse(strValue, out Guid guidResult))
                    {
                        return guidResult;
                    }

                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} due to incorrect format.");
                }

#pragma warning disable RS0030
                if (targetType == typeof(DateTime))
                {
                    if (DateTime.TryParse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dateTimeResult))
                    {
                        return dateTimeResult;
                    }

                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} due to incorrect format.");
                }
#pragma warning restore RS0030

                if (targetType == typeof(TimeSpan))
                {
                    if (TimeSpan.TryParse(strValue, CultureInfo.InvariantCulture, out TimeSpan timeSpanResult))
                    {
                        return timeSpanResult;
                    }

                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} due to incorrect format.");
                }

                // Consider adding support for other numeric types in the future:
                // if (targetType == typeof(byte) && byte.TryParse(strValue, out byte byteResult)) return byteResult;
                // if (targetType == typeof(sbyte) && sbyte.TryParse(strValue, out sbyte sbyteResult)) return sbyteResult;
                // if (targetType == typeof(short) && short.TryParse(strValue, out short shortResult)) return shortResult;
                // if (targetType == typeof(ushort) && ushort.TryParse(strValue, out ushort ushortResult)) return ushortResult;
                // if (targetType == typeof(long) && long.TryParse(strValue, out long longResult)) return longResult;
                // if (targetType == typeof(ulong) && ulong.TryParse(strValue, out ulong ulongResult)) return ulongResult;
                // if (targetType == typeof(float) && float.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatResult)) return floatResult;
                // if (targetType == typeof(double) && double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleResult)) return doubleResult;
                // if (targetType == typeof(decimal) && decimal.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalResult)) return decimalResult;
            }

            if (value is IConvertible convertible)
            {
                try
                {
                    object? result = Convert.ChangeType(convertible, targetType, CultureInfo.InvariantCulture);
                    return result;
                }
                catch (OverflowException ex)
                {
                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} due to overflow.", ex);
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} as the conversion is invalid.", ex);
                }
                catch (FormatException ex)
                {
                    throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType} due to incorrect format.", ex);
                }
            }

            throw new InvalidOperationException($"Cannot convert value '{value}' to {targetType}.");
        }
    }
}
