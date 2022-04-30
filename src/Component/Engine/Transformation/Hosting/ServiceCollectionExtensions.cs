// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Engine.Transformation.Service.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kaylumah.Ssg.Engine.Transformation.Hosting;
public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddTransformationEngine(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

        services.AddTransient<IPlugin, SeoPlugin>();
        services.AddTransient<IPlugin, FeedPlugin>();

        services.AddSingleton<ITransformationEngine, TransformationEngine>();
        return services;
    }
}
