using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public interface IFileMetadataParser
    {
        Metadata<FileMetaData> Parse(MetadataCriteria criteria);
    }
}