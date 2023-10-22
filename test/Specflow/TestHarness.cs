// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Utilities;

public sealed class TestHarness
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly ProxyGenerator ProxyGenerator = new();
    private readonly IReadOnlyList<IInterceptor> _interceptors;

    public TestHarness(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _interceptors = new ReadOnlyCollection<IInterceptor>(serviceProvider.GetServices<IAsyncInterceptor>().ToInterceptors());
    }

    public async Task TestService<T>(Func<T, Task> scenario) where T : class
    {
        T proxy = GetProxy<T>();
        await TestService(proxy, scenario).ConfigureAwait(false);
    }

    private T GetProxy<T>() where T : class
    {
        Type targetType = typeof(T);
        T instance = _serviceProvider.GetRequiredService<T>();
        if (targetType.IsInterface)
        {
            T proxy = ProxyGenerator.CreateInterfaceProxyWithTarget(instance, _interceptors.ToArray());
            return proxy;
        }
        else
        {
            System.Reflection.ConstructorInfo constructor = targetType.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
            {
                T proxy = ProxyGenerator.CreateClassProxyWithTarget(instance, _interceptors.ToArray());
                return proxy;
            }
        }

        // Fallback to the instance from DI without interception
        return instance;
    }

    private static async Task TestService<T>(T instance, Func<T, Task> scenario) where T : class
    {
        await scenario(instance).ConfigureAwait(false);
    }
}
