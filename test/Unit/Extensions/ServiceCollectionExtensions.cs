// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTestLogging(this IServiceCollection serviceCollection)
        {
            // FakeLogCollector collector = serviceProvider.GetRequiredService<FakeLogCollector>();

            serviceCollection.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddFakeLogging(options =>
                {
                });
            });

            return serviceCollection;
        }
    }
}