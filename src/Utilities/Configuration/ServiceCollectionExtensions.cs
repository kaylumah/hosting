// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
    // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Options.ConfigurationExtensions/src/OptionsConfigurationServiceCollectionExtensions.cs
    // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Options/src/OptionsServiceCollectionExtensions.cs
    // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Options.DataAnnotations/src/OptionsBuilderDataAnnotationsExtensions.cs
    // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Options/src/OptionsFactory.cs
    // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Options/src/OptionsBuilder.cs
    // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Options/src/ValidateOptions.cs


    public static IServiceCollection SetupOptions<TOptions>(this IServiceCollection services, IConfiguration configuration) where TOptions : class
    {
        var key = typeof(TOptions).Name;
        return services
            .SetupOptions<TOptions>(configuration, key);
    }

    public static IServiceCollection SetupOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, string key) where TOptions : class
    {
        var section = configuration.GetRequiredSection(key);
        var configureDelegate = (TOptions options) => section.Bind(options);
        return services
            .SetupOptions(configureDelegate);
    }

    public static IServiceCollection SetupOptions<TOptions>(this IServiceCollection services, Action<TOptions> configureDelegate) where TOptions : class
    {
        services.Configure(configureDelegate);
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);
        return services;
    }
}
