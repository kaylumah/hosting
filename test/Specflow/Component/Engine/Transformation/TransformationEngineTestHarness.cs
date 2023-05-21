// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Kaylumah.Ssg.Engine.Transformation.Hosting;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Microsoft.Extensions.DependencyInjection;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Test.Utilities;

namespace Test.Specflow.Component.Engine.Transformation;

public sealed class TransformationEngineTestHarness
{
    public TestHarnessBuilder TestHarnessBuilder { get; }

    public TransformationEngineTestHarness(MockFileSystem mockFileSystem)
    {
        TestHarnessBuilder = TestHarnessBuilder.Create()
            .Register((serviceCollection, configuration) =>
            {
                serviceCollection.AddSingleton<IFileSystem>(mockFileSystem);
                serviceCollection.AddSingleton<IMetadataProvider, YamlFrontMatterMetadataProvider>();
                serviceCollection.AddSingleton<IYamlParser, YamlParser>();
                serviceCollection.AddTransformationEngine(configuration);
            });
    }
    
    public async Task TestTransformationEngine(Func<ITransformationEngine, Task> scenario)
    {
        var testHarness = TestHarnessBuilder.Build();
        await testHarness.TestService(scenario).ConfigureAwait(false);
    }
}
