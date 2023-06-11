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
        var configurationBuilder = new ConfigurationBuilder();
        foreach (var configurationRegistration in _configurationRegistrationActions)
        {
            configurationRegistration(configurationBuilder);
        }

        var configuration = configurationBuilder.Build();

        var services = new ServiceCollection();
        foreach (var serviceRegistrationAction in _serviceRegistrationActions)
        {
            serviceRegistrationAction(services, configuration);
        }

        var serviceProvider = services.BuildServiceProvider(validateScopes: true);
        var testHarness = serviceProvider.GetRequiredService<TestHarness>();
        return testHarness;
    }
}
