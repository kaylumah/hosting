// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class StronglyTypedIdHelper<T> where T : struct
    {
        public Func<object, T> FromObject
        { get; }

        public Func<T, object> ToObject
        { get; }
        
        public Type UnderlyingType
        { get; }
        
        public StronglyTypedIdHelper()
        {
            Type strongIdType = typeof(T);
            UnderlyingType = GetUnderlyingType(strongIdType);

            MethodInfo[] methods = strongIdType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            IEnumerable<MethodInfo> implicitOperatorMethods = methods.Where(IsImplicitOperator).ToList();

            MethodInfo? fromMethod = implicitOperatorMethods
                .SingleOrDefault(method => IsSpecificOperator(method, strongIdType, UnderlyingType));
            MethodInfo? toMethod = implicitOperatorMethods
                .SingleOrDefault(method => IsSpecificOperator(method, UnderlyingType, strongIdType));

            if (fromMethod == null || toMethod == null)
            {
                throw new InvalidOperationException($"Type {strongIdType.Name} must have implicit conversions to and from {UnderlyingType}.");
            }

            FromObject = (object value) =>
            {
                object[] arguments = new object[] { value };
                T result = (T)fromMethod.Invoke(null, arguments)!;
                return result;
            };
            
            ToObject = (T value) =>
            {
                object[] arguments = new object[] { value };
                object result = toMethod.Invoke(null, arguments)!;
                return result;
            };
        }
        
        static bool IsImplicitOperator(MethodInfo methodInfo)
        {
            bool result = methodInfo is { IsSpecialName: true, Name: "op_Implicit" };
            return result;
        }

        static bool IsSpecificOperator(MethodInfo methodInfo, Type returnType, Type parameterType)
        {
            bool returnTypeMatches = methodInfo.ReturnType == returnType;

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            ParameterInfo parameterInfo = parameterInfos.Single();
            bool parameterMatches = parameterInfo.ParameterType == parameterType;

            bool result = returnTypeMatches && parameterMatches;
            return result;
        }
        
        static Type GetUnderlyingType(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo constructor = constructors.Single();

            ParameterInfo[] parameters = constructor.GetParameters();
            ParameterInfo parameterInfo = parameters.Single();

            Type result = parameterInfo.ParameterType;
            return result;
        }
    }
    
    public class StronglyTypedIdJsonConverter<T> : JsonConverter<T> where T : struct
    {
        readonly StronglyTypedIdHelper<T> _StronglyTypedIdHelper;
        
        public StronglyTypedIdJsonConverter()
        {
            _StronglyTypedIdHelper = new StronglyTypedIdHelper<T>();
        }
        
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Type targetType = _StronglyTypedIdHelper.UnderlyingType;
            object value = targetType switch
            {
                _ when targetType == typeof(string) => reader.GetString() ?? string.Empty,
                _ when targetType ==  typeof(Guid) => reader.GetGuid(),
                _ when targetType == typeof(int) => reader.GetInt32(),
                _ => throw new JsonException($"Unsupported ID type {targetType}.")
            };

            T result = _StronglyTypedIdHelper.FromObject(value);
            return result;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            Type targetType = _StronglyTypedIdHelper.UnderlyingType;
            object objValue = _StronglyTypedIdHelper.ToObject(value);

            switch (objValue)
            {
                case string strVal:
                    writer.WriteStringValue(strVal);
                    break;
                case Guid guidVal:
                    writer.WriteStringValue(guidVal);
                    break;
                case int intVal:
                    writer.WriteNumberValue(intVal);
                    break;
                default:
                    throw new JsonException($"Unsupported ID type {targetType}.");
            }
        }

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? input = reader.GetString();
            if (string.IsNullOrEmpty(input))
            {
                throw new JsonException("Property name cannot be null or empty.");
            }

            // TODO what happens if its not a string?
            T result = _StronglyTypedIdHelper.FromObject(input);
            return result;
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            object x = _StronglyTypedIdHelper.ToObject(value);
            string xAsString = x.ToString()!;
            writer.WritePropertyName(xAsString);
        }
    }
    
    public class StronglyTypedIdYamlConverter<T> : IYamlTypeConverter where T : struct
    {
        readonly StronglyTypedIdHelper<T> _StronglyTypedIdHelper;
        
        public StronglyTypedIdYamlConverter()
        {
            _StronglyTypedIdHelper = new StronglyTypedIdHelper<T>();
        }
        
        bool IYamlTypeConverter.Accepts(Type type)
        {
            bool result = type == typeof(T);
            return result;
        }

        object? IYamlTypeConverter.ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            if (parser.Current is not Scalar scalar || string.IsNullOrEmpty(scalar.Value))
            {
                throw new YamlException("Invalid or missing YAML scalar value for strongly-typed ID.");
            }
            
            object result = _StronglyTypedIdHelper.FromObject(scalar.Value);
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