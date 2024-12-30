// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Castle.DynamicProxy;
using Kaylumah.Ssg.Utilities.Common;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterImplementationsAsSingleton<T>(this IServiceCollection serviceCollection)
        {
            Type abstractionType = typeof(T);
            Assembly assembly = abstractionType.Assembly;
            serviceCollection.RegisterImplementationsAsSingleton<T>(assembly);
            return serviceCollection;
        }

        public static IServiceCollection RegisterImplementationsAsSingleton<T>(this IServiceCollection serviceCollection, Assembly assembly)
        {
            Type abstractionType = typeof(T);
            Type[] typesToAdd = assembly.GetImplementationsForType(abstractionType);

            foreach (Type concretion in typesToAdd)
            {
                serviceCollection.AddSingleton(concretion);
                serviceCollection.AddSingleton(abstractionType, sp => sp.GetRequiredService(concretion));
            }

            return serviceCollection;
        }

        public static IServiceCollection AddProxiedService<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddSingleton<TImplementation>();
            services.AddSingleton(provider =>
            {
                IAsyncInterceptor interceptor = ActivatorUtilities.CreateInstance<LoggingInterceptor>(provider);
                IAsyncInterceptor[] interceptors = [interceptor];

                TImplementation implementation = provider.GetRequiredService<TImplementation>();
                TInterface result = Proxy.ProxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptors);
                return result;
            });

            return services;
        }

#pragma warning disable CA1848
    }
}