// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
}