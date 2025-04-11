// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace Test.Unit.Core
{
    public class GetValuesTests
    {
        static readonly MethodInfo _GetValuesMethod;
        static readonly ConcurrentDictionary<Type, MethodInfo> _GetValuesMethods;

        static GetValuesTests()
        {
            Type type = typeof(DictionaryExtensions);

            _GetValuesMethod = type.GetMethod("GetValues")!;
            Debug.Assert(_GetValuesMethod != null);

            _GetValuesMethods = new();
            GetValuesMethod(typeof(string));
        }
        
        static MethodInfo GetValuesMethod(Type target)
        {
            MethodInfo methodInfo = _GetValuesMethods.GetOrAdd(target, (cacheKey) =>
            {
                MethodInfo result = _GetValuesMethod.MakeGenericMethod(cacheKey);
                return result;
            });

            return methodInfo;
        }
        
        [Fact]
        public void Test_GetValues_ArgumentNullExceptionForNullDictionary()
        {
            Dictionary<string, object?>? target = null;
            Assert.Throws<ArgumentNullException>(() => target!.GetValues<string>("some-key"));
        }

        [Fact]
        public void Test_GetValues_ArgumentNullExceptionForNullKey()
        {
            Dictionary<string, object?> target = new();
            Assert.Throws<ArgumentNullException>(() => target.GetValues<string>(null!));
        }
    }
}