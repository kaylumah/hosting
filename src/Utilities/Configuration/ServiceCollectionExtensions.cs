// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kaylumah.Ssg.Utilities.Configuration;

public static partial class ServiceCollectionExtensions
{
    /*
    public static IServiceCollection AddExample(this IServiceCollection services, IConfiguration config)
    {
        services.AddExample(options => config.GetSection(ExampleOptions.DefaultConfigurationSectionName).Bind(options));
        return services;
    }

    public static IServiceCollection AddExample(this IServiceCollection services, Action<ExampleOptions> configureDelegate)
    {
        services.Configure(configureDelegate);
        return services;
    }
    */

    public static IServiceCollection AddOptionsV2<TOptions>(this IServiceCollection services, IConfiguration configuration)
    {
        var configuration2 = configuration.GetRequiredSection(string.Empty);
        // services.Configure<DemoOptions>(configuration.GetSection(DemoOptions.DefaultConfigurationSectionName));
        return services;
    }

    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
    {
        // services.Configure<DemoOptions>(configuration.GetSection(DemoOptions.DefaultConfigurationSectionName));
        return services;
    }

    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services, Action<TOptions> configureDelegate) where TOptions : class
    {
        services.Configure(configureDelegate);
        return services;
    }

    public static IConfigurationSection GetExistingSectionOrThrow(this IConfiguration configuration, string key)
    {
        var configurationSection = configuration.GetSection(key);

        if (!configurationSection.Exists())
        {
            throw configuration switch
            {
                IConfigurationRoot configurationIsRoot => new ArgumentException($"Section with key '{key}' does not exist. Existing values are: {configurationIsRoot.GetDebugView()}", nameof(key)),
                IConfigurationSection configurationIsSection => new ArgumentException($"Section with key '{key}' does not exist at '{configurationIsSection.Path}'. Expected configuration path is '{configurationSection.Path}'", nameof(key)),
                _ => new ArgumentException($"Failed to find configuration at '{configurationSection.Path}'", nameof(key))
            };
        }

        return configurationSection;
    }
}
