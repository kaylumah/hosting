// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Time.Testing;

namespace Kaylumah.Ssg.iFX.Test
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Replaces <see cref="TimeProvider"/> with ...
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceTimeProvider(this IServiceCollection services)
        {
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<FakeTimeProvider>();
            services.AddSingleton<TimeProvider>(serviceProvider => serviceProvider.GetRequiredService<FakeTimeProvider>());
            
            return services;
        }

        /// <summary>
        /// Replaces <see cref="ILogger"/> with ...
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ReplaceLogger(this IServiceCollection services)
        {
            // FakeLogCollector collector = serviceProvider.GetRequiredService<FakeLogCollector>();
            
            void ConfigureFakeLogger(FakeLogCollectorOptions options)
            {
                // Empty on purpose for now   
            }
            
            void ConfigureLogging(ILoggingBuilder loggingBuilder)
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddFakeLogging(ConfigureFakeLogger);
            }
            
            services.AddLogging(ConfigureLogging);

            return services;
        }
    }
    
    public static class TestHarnessBuilderExtensions
    {
        public static TestHarnessBuilder SetupTimeProvider(this TestHarnessBuilder builder)
        {
            void ReplaceTimeProvider(IServiceCollection services)
            {
                services.ReplaceTimeProvider();
            }

            builder.Register(ReplaceTimeProvider);
            return builder;
        }

        public static TestHarnessBuilder SetupLogger(this TestHarnessBuilder builder)
        {
            void ReplaceLogger(IServiceCollection services)
            {
                services.ReplaceLogger();
            }
            
            builder.Register(ReplaceLogger);
            return builder;
        }
    }
}