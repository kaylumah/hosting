// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using TechTalk.SpecFlow;

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
            var directoryConfig = new DirectoryConfiguration()
            {
                SourceDirectory = "_site",
                LayoutsDirectory = "_layouts",
                TemplateDirectory = "_includes"
            };
            var renderResult = await transformationEngine.Render(
                    directoryConfig,
                    new MetadataRenderRequest[] { })
                .ConfigureAwait(false);
        }

        await _transformationEngineTestHarness.TestTransformationEngine(TestScenario).ConfigureAwait(false);
    }
}
