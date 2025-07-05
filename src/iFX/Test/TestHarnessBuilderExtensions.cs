// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace Kaylumah.Ssg.iFX.Test
{
    public static class TestHarnessBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="TestHarnessBuilder"/> to configure.</param>
        /// <returns>The <see cref="TestHarnessBuilder"/> so that additional calls can be chained.</returns>
        /// <example>
        /// <code>
        /// TestHarness harness = TestHarnessBuilder.Create()
        ///     .SetupTimeProvider()
        ///     .Build(out IServiceProvider serviceProvider);
        /// FakeTimeProvider fakeTimeProvider = serviceProvider.GetRequiredService&lt;FakeTimeProvider&gt;();
        /// DateTimeOffset time = DateTimeOffset.Parse("2025-06-28T12:00:00Z", CultureInfo.InvariantCulture);
        /// fakeTimeProvider.SetUtcNow(time);
        /// </code>
        /// </example>
        public static TestHarnessBuilder SetupTimeProvider(this TestHarnessBuilder builder)
        {
            void ReplaceTimeProvider(IServiceCollection services)
            {
                services.ReplaceTimeProvider();
            }

            builder.Register(ReplaceTimeProvider);
            return builder;
        }

        public static TestHarnessBuilder SetupTimeProvider(this TestHarnessBuilder builder, FakeTimeProvider fakeTimeProvider)
        {
            void ReplaceTimeProvider(IServiceCollection services)
            {
                services.ReplaceTimeProvider(fakeTimeProvider);
            }

            builder.Register(ReplaceTimeProvider);
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="TestHarnessBuilder"/> to configure.</param>
        /// <returns>The <see cref="TestHarnessBuilder"/> so that additional calls can be chained.</returns>
        /// <example>
        /// <code>
        /// TestHarness harness = TestHarnessBuilder.Create()
        ///     .SetupLogger()
        ///     .Build(out IServiceProvider serviceProvider);
        /// FakeLogCollector logCollector = serviceProvider.GetRequiredService&lt;FakeLogCollector&gt;();
        /// </code>
        /// </example>
        public static TestHarnessBuilder SetupLogger(this TestHarnessBuilder builder)
        {
            void ReplaceLogger(IServiceCollection services)
            {
                services.ReplaceLogger();
            }

            builder.Register(ReplaceLogger);
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="TestHarnessBuilder"/> to configure.</param>
        /// <returns>The <see cref="TestHarnessBuilder"/> so that additional calls can be chained.</returns>
        /// <example>
        /// <code>
        /// TestHarness harness = TestHarnessBuilder.Create()
        ///     .SetupFileSystem()
        ///     .Build(out IServiceProvider serviceProvider);
        /// MockFileSystem mockFileSystem = serviceProvider.GetRequiredService&lt;MockFileSystem&gt;();
        /// </code>
        /// </example>
        public static TestHarnessBuilder SetupFileSystem(this TestHarnessBuilder builder)
        {
            void ReplaceFileSystem(IServiceCollection services)
            {
                services.ReplaceFileSystem();
            }

            builder.Register(ReplaceFileSystem);
            return builder;
        }

        public static TestHarnessBuilder SetupFileSystem(this TestHarnessBuilder builder, MockFileSystem mockFileSystem)
        {
            void ReplaceFileSystem(IServiceCollection services)
            {
                services.ReplaceFileSystem(mockFileSystem);
            }

            builder.Register(ReplaceFileSystem);
            return builder;
        }
    }
}