// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kaylumah.Ssg.iFX.Test
{
    public sealed class TestHarnessBuilder
    {
        readonly List<Action<IConfigurationBuilder>> _ConfigurationRegistrationActions;
        readonly List<Action<IServiceCollection, IConfiguration>> _ServiceRegistrationActions;

        TestHarnessBuilder()
        {
            _ConfigurationRegistrationActions = new();
            _ServiceRegistrationActions = new();
            Register((serviceCollection) =>
            {
                serviceCollection.AddSingleton<TestHarness>();
            });
        }

        public static TestHarnessBuilder Create()
        {
            TestHarnessBuilder result = new TestHarnessBuilder();
            return result;
        }

        public TestHarnessBuilder Configure(Action<IConfigurationBuilder> configurationRegistrationAction)
        {
            _ConfigurationRegistrationActions.Add(configurationRegistrationAction);
            return this;
        }

        public TestHarnessBuilder Register(Action<IServiceCollection, IConfiguration> serviceRegistrationAction)
        {
            _ServiceRegistrationActions.Add(serviceRegistrationAction);
            return this;
        }

        public TestHarnessBuilder Register(Action<IServiceCollection> serviceRegistrationAction)
        {
            TestHarnessBuilder result = Register((serviceCollection, _) => serviceRegistrationAction(serviceCollection));
            return result;
        }

        public TestHarness Build()
        {
            TestHarness harness = Build(out _, out _);
            return harness;
        }

        public TestHarness Build(out IServiceProvider serviceProvider)
        {
            TestHarness harness = Build(out serviceProvider, out _);
            return harness;
        }

        public TestHarness Build(out IConfigurationRoot configuration)
        {
            TestHarness harness = Build(out _, out configuration);
            return harness;
        }

        public TestHarness Build(out IServiceProvider serviceProvider, out IConfigurationRoot configuration)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            foreach (Action<IConfigurationBuilder> configurationRegistration in _ConfigurationRegistrationActions)
            {
                configurationRegistration(configurationBuilder);
            }

            configuration = configurationBuilder.Build();

            ServiceCollection services = new ServiceCollection();
            foreach (Action<IServiceCollection, IConfiguration> serviceRegistrationAction in _ServiceRegistrationActions)
            {
                serviceRegistrationAction(services, configuration);
            }

            ServiceProviderOptions serviceProviderOptions = new ServiceProviderOptions();
            serviceProviderOptions.ValidateScopes = true;
            serviceProviderOptions.ValidateOnBuild = true;
            serviceProvider = services.BuildServiceProvider(serviceProviderOptions);

            TestHarness testHarness = serviceProvider.GetRequiredService<TestHarness>();
            return testHarness;
        }
    }
}
