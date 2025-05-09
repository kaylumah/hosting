// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public interface IFrontMatterMetadataProvider
    {
        ParsedFile<T> Retrieve<T>(string contents);
    }
}
