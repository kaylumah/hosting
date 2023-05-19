// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
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
        var instance = _serviceProvider.GetRequiredService<T>();
        var proxy = ProxyGenerator.CreateInterfaceProxyWithTarget(instance, _interceptors.ToArray());
        await TestService(proxy, scenario).ConfigureAwait(false);
    }

    private static async Task TestService<T>(T instance, Func<T, Task> scenario) where T : class
    {
        await scenario(instance).ConfigureAwait(false);
    }
}
