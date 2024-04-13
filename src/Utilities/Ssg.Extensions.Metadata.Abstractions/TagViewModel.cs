// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class TagViewModel
    {
        public string Id
        { get; init; }

        public string DisplayName
        { get; init; }

        public int Size
        { get; init; }

        public TagViewModel(string id, string displayName, int size)
        {
            Id = id;
            DisplayName = displayName;
            Size = size;
        }
    }
}