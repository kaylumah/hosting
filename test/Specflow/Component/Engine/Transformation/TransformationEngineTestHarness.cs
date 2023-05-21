// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Test.Utilities;

namespace Test.Specflow.Component.Engine.Transformation;

public sealed class TransformationEngineTestHarness
{
    public TestHarnessBuilder TestHarnessBuilder { get; }

    public TransformationEngineTestHarness()
    {
        TestHarnessBuilder = TestHarnessBuilder.Create()
            .Register((serviceCollection, configuration) => { });
    }
    
    public async Task TestTransformationEngine(Func<ITransformationEngine, Task> scenario)
    {
        var testHarness = TestHarnessBuilder.Build();
        await testHarness.TestService(scenario).ConfigureAwait(false);
    }
}
