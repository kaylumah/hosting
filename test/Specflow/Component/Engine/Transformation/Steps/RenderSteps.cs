// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Test.Specflow.Component.Engine.Transformation.Steps;

[Binding]
[Scope(Feature = "TransformationEngine Render")]
public sealed class RenderSteps
{
    private readonly TransformationEngineTestHarness _transformationEngineTestHarness;

    public RenderSteps(TransformationEngineTestHarness transformationEngineTestHarness)
    {
        _transformationEngineTestHarness = transformationEngineTestHarness;
    }

    [When]
    public async Task WhenTheTransformationEngineIsCalled()
    {
        async Task TestScenario(ITransformationEngine transformationEngine)
        {
            var renderResult = await transformationEngine.Render(
                    new DirectoryConfiguration(),
                    new MetadataRenderRequest[] { })
                .ConfigureAwait(false);
        }

        await _transformationEngineTestHarness.TestTransformationEngine(TestScenario).ConfigureAwait(false);
    }
}
