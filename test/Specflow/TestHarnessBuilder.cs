// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Utilities;

public sealed class TestHarnessBuilder
{
    private readonly List<Action<IConfigurationBuilder>> _configurationRegistrationActions = new();
    private readonly List<Action<IServiceCollection, IConfiguration>> _serviceRegistrationActions = new();

    private TestHarnessBuilder()
    {
        Register((serviceCollection) =>
        {
            serviceCollection.AddSingleton<TestHarness>();
        });
    }

    public static TestHarnessBuilder Create()
    {
        return new TestHarnessBuilder();
    }

    public TestHarnessBuilder Configure(Action<IConfigurationBuilder> configurationRegistrationAction)
    {
        _configurationRegistrationActions.Add(configurationRegistrationAction);
        return this;
    }

    public TestHarnessBuilder Register(Action<IServiceCollection, IConfiguration> serviceRegistrationAction)
    {
        _serviceRegistrationActions.Add(serviceRegistrationAction);
        return this;
    }

    public TestHarnessBuilder Register(Action<IServiceCollection> serviceRegistrationAction)
    {
        return Register((serviceCollection, _) => serviceRegistrationAction(serviceCollection));
    }

    public TestHarness Build()
    {
        ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        foreach (Action<IConfigurationBuilder> configurationRegistration in _configurationRegistrationActions)
        {
            configurationRegistration(configurationBuilder);
        }

        IConfigurationRoot configuration = configurationBuilder.Build();

        ServiceCollection services = new ServiceCollection();
        foreach (Action<IServiceCollection, IConfiguration> serviceRegistrationAction in _serviceRegistrationActions)
        {
            serviceRegistrationAction(services, configuration);
        }

        ServiceProvider serviceProvider = services.BuildServiceProvider(validateScopes: true);
        TestHarness testHarness = serviceProvider.GetRequiredService<TestHarness>();
        return testHarness;
    }
}
