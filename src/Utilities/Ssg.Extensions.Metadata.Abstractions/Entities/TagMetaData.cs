// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("TagMetaData '{Name}'")]
    public class TagMetaData
    {
        public TagId Id
        { get; set; } = null!;
        public string Name
        { get; set; } = null!;
        public string Description
        { get; set; } = null!;
        public string Icon
        { get; set; } = null!;
    }
}
