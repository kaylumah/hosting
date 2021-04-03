// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Engine.Transformation.Interface
{
    public interface IMetadataRenderer
    {
        Task<MetadataRenderResult[]> Render(MetadataRenderRequest[] requests);
    }
}