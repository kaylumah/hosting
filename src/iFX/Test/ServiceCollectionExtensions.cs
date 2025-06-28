// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
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
        /// Replaces the default <see cref="TimeProvider"/> registration with a testable <see cref="FakeTimeProvider"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
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
        /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection ReplaceLogger(this IServiceCollection services)
        {
            void ConfigureLogger(FakeLogCollectorOptions options, TimeProvider timeProvider)
            {
                options.TimeProvider = timeProvider;
            }
            
            void ConfigureLogging(ILoggingBuilder loggingBuilder)
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddFakeLogging();

                loggingBuilder.Services.AddOptions<FakeLogCollectorOptions>()
                    .Configure<TimeProvider>(ConfigureLogger);
            }
            
            services.TryAddSingleton(TimeProvider.System);
            services.AddLogging(ConfigureLogging);

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection ReplaceFileSystem(this IServiceCollection services)
        {
            MockFileSystem MockFileSystemFactory(IServiceProvider serviceProvider)
            {
                TimeProvider timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
                MockFileSystem mockFileSystem = new MockFileSystem();
                mockFileSystem.MockTime(() => timeProvider.GetUtcNow().UtcDateTime);
                return mockFileSystem;
            }
            
            services.RemoveAll<IFileSystem>();
            services.TryAddSingleton(TimeProvider.System);
            services.AddSingleton(MockFileSystemFactory);
            services.AddSingleton<IFileSystem>(serviceProvider => serviceProvider.GetRequiredService<MockFileSystem>());
    
            return services;
        }
    }
}