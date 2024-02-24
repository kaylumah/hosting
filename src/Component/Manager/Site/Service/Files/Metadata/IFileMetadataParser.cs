// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Files.Metadata
{
    public interface IFileMetadataParser
    {
        // rename Metadata to FileResponse?
        Metadata<FileMetaData> Parse(MetadataCriteria criteria);
    }
}
