// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
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
    }
}