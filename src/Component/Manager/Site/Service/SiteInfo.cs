// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class SiteInfo
    {
        public string Lang { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Collections Collections { get; set; } = new Collections();
        public HashSet<string> SupportedFileExtensions { get; set; } = new HashSet<string>();
        public HashSet<string> SupportedDataFileExtensions { get; set; } = new HashSet<string>();

    }
}
