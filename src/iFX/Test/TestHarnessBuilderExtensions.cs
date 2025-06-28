// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Kaylumah.Ssg.iFX.Test
{
    public static class TestHarnessBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// TestHarness harness = TestHarnessBuilder.Create()
        ///                         .SetupTimeProvider()
        ///                         .Build(out IServiceProvider serviceProvider);
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