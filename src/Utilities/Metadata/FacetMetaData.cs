// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public class FacetMetaData
    {
        public string Id
        { get; init; }

        public string DisplayName
        { get; init; }

        public string Description
        { get; init; }

        public int Size
        { get; init; }

        public FacetMetaData(string id, string displayName, string description, int size)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Size = size;
        }
    }
}