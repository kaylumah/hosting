// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("AuthorMetaData '{FullName}'")]
    public class AuthorMetaData
    {
        public AuthorId Id
        { get; set; } = null!;
        public string FullName
        { get; set; } = null!;
        public string Email
        { get; set; } = null!;
        public string Uri
        { get; set; } = null!;
        public string Picture
        { get; set; } = null!;
        public Links Links
        { get; set; } = new();
    }
}