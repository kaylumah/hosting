// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

public interface ITransformationEngine
{
    Task<MetadataRenderResult[]> Render(DirectoryConfiguration directoryConfiguration, MetadataRenderRequest[] requests);
}
