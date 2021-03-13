using System;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public interface IMetadataProvider
    {
        Metadata<T> Retrieve<T>(string contents);
    }
}
