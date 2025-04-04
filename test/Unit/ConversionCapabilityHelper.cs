// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Test.Unit
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
            sb.AppendLine(CultureInfo.InvariantCulture, $"{"Type",-30} {"TypeConverter",-15} {"IConvertible",-15}");

            // Rows
            foreach (Type type in types)
            {
                string displayName = GetFriendlyTypeName(type);
                bool canConvert = CanConvertFromString(type);
                bool convertible = ImplementsIConvertible(type);

                sb.AppendLine(CultureInfo.InvariantCulture, $"{displayName,-30} {(canConvert ? "✅" : "❌"),-15} {(convertible ? "✅" : "❌"),-15}");
            }

            string result = sb.ToString();
            return result;
        }
    }
}