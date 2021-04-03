// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Threading.Tasks;

namespace Kaylumah.Ssg.Engine.Transformation.Interface
{
    public interface ITransformationEngine : IMetadataRenderer
    {
    }

    public interface IMetadataRenderer
    {
        Task<MetadataRenderResult[]> Render(MetadataRenderRequest[] requests);
    }

    public class MetadataRenderRequest
    {
        public IMetadata Metadata { get;set; }
        public string Template { get;set; }
        public string Content { get;set; }
    }

    public class MetadataRenderResult
    {
        public string Content { get; set; }
    }
    
    public interface IMetadata
    {
        string Title { get; }
        string Description { get; }
        string Language { get; }
        string Author { get; }
        string Url { get; }
    }
}
