// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("OrganizationMetaData '{FullName}'")]
    public class OrganizationMetaData
    {
        public OrganizationId Id
        { get; set; } = null!;
        public string FullName
        { get; set; } = null!;
        public string Linkedin
        { get; set; } = null!;
        public string Twitter
        { get; set; } = null!;
        public string Logo
        { get; set; } = null!;
        public DateTimeOffset Founded
        { get; set; }
    }
}