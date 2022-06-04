﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Kaylumah.Ssg.Manager.Site.Service;

public class SiteInfo
{
    public string Lang { get; set; }
    public string BaseUrl { get; set; }
    public string Url { get; set; }
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public Collections Collections { get; set; } = new Collections();
    public HashSet<string> SupportedFileExtensions { get; set; } = new HashSet<string>();
    public HashSet<string> SupportedDataFileExtensions { get; set; } = new HashSet<string>();

}
