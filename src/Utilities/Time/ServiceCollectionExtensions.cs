// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Kaylumah.Ssg.Utilities.Time
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemClock(this IServiceCollection services)
        {
            services.AddTransient<ISystemClock, SystemClock>();
            return services;
        }
    }
}
