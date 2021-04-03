// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Engine.Transformation.Service
{
    public class TransformationEngine : ITransformationEngine
    {
        public Task<MetadataRenderResult[]> Render(MetadataRenderRequest[] requests)
        {
            throw new System.NotImplementedException();
        }
    }
}
