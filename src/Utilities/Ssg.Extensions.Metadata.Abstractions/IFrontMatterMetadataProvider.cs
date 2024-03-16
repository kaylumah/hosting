// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Metadata.Abstractions
{
    public interface IFrontMatterMetadataProvider
    {
        ParsedFile<T> Retrieve<T>(string contents);
    }
}
