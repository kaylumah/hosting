// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

namespace Kaylumah.Ssg.iFX.Test
{
    public static class TestHarnessBuilderExtensions
    {
        public static TestHarnessBuilder SetupTimeProvider(this TestHarnessBuilder builder)
        {
            void ReplaceTimeProvider(IServiceCollection services)
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<FakeTimeProvider>();
                services.AddSingleton<TimeProvider>(serviceProvider => serviceProvider.GetRequiredService<FakeTimeProvider>());
            }

            builder.Register(ReplaceTimeProvider);
            return builder;
        }
    }
}