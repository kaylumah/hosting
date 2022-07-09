// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Moq;

namespace Test.Specflow.Utilities;

public class TransformationEngineMock : StrictMock<ITransformationEngine>
{
    public TransformationEngineMock()
    {
        SetupRender();
    }

    public void SetupRender()
    {
        Setup(transformationEngine =>
                transformationEngine.Render(It.IsAny<MetadataRenderRequest[]>()))
            .ReturnsAsync((MetadataRenderRequest[] requests) =>
            {
                var result = new List<MetadataRenderResult>();
                foreach (var renderRequest in requests)
                {
                    result.Add(new MetadataRenderResult()
                    {
                        Content = string.Empty
                    });
                }
                return result.ToArray();
            });
    }
}
