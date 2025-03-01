// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class StronglyTypedIdYamlConverter<T> : IYamlTypeConverter where T : struct
    {
        readonly StronglyTypedIdHelper<T> _StronglyTypedIdHelper;

        public StronglyTypedIdYamlConverter()
        {
            _StronglyTypedIdHelper = new StronglyTypedIdHelper<T>();
        }

        bool IYamlTypeConverter.Accepts(Type type)
        {
            Type? nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType == typeof(T))
            {
                return true;
            }

            bool result = type == typeof(T);
            return result;
        }

        object? IYamlTypeConverter.ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not Scalar scalar || string.IsNullOrEmpty(scalar.Value))
            {
                throw new YamlException("Invalid or missing YAML scalar value for strongly-typed ID.");
            }

            parser.MoveNext();

            Type targetType = _StronglyTypedIdHelper.UnderlyingType;
            object? converted = scalar.Value.ConvertValue(targetType);
            Debug.Assert(converted != null);

            object result = _StronglyTypedIdHelper.FromObject(converted);
            return result;
        }

        void IYamlTypeConverter.WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value), "Cannot serialize a null strongly-typed ID.");
            }

            object convertedValue = _StronglyTypedIdHelper.ToObject((T)value);
            string convertedValueString = convertedValue.ToString() ?? throw new InvalidOperationException("Conversion to string resulted in null.");
            Scalar scalarValue = new Scalar(convertedValueString);
            emitter.Emit(scalarValue);
        }
    }
}