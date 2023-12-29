// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Utilities
{
    public sealed class TestHarness
    {
        readonly IServiceProvider _ServiceProvider;
        static readonly ProxyGenerator _ProxyGenerator = new();
        readonly IReadOnlyList<IInterceptor> _Interceptors;

        public TestHarness(IServiceProvider serviceProvider)
        {
            _ServiceProvider = serviceProvider;
            _Interceptors = new ReadOnlyCollection<IInterceptor>(serviceProvider.GetServices<IAsyncInterceptor>().ToInterceptors());
        }

        public async Task TestService<T>(Func<T, Task> scenario) where T : class
        {
            T proxy = GetProxy<T>();
            await TestService(proxy, scenario).ConfigureAwait(false);
        }

        T GetProxy<T>() where T : class
        {
            Type targetType = typeof(T);
            T instance = _ServiceProvider.GetRequiredService<T>();
            if (targetType.IsInterface)
            {
                T proxy = _ProxyGenerator.CreateInterfaceProxyWithTarget(instance, _Interceptors.ToArray());
                return proxy;
            }
            else
            {
                System.Reflection.ConstructorInfo constructor = targetType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    T proxy = _ProxyGenerator.CreateClassProxyWithTarget(instance, _Interceptors.ToArray());
                    return proxy;
                }
            }

            // Fallback to the instance from DI without interception
            return instance;
        }

        static async Task TestService<T>(T instance, Func<T, Task> scenario) where T : class
        {
            await scenario(instance).ConfigureAwait(false);
        }
    }
}
