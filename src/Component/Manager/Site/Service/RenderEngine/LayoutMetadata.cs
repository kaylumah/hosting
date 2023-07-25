// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using YamlDotNet.Serialization;

namespace Kaylumah.Ssg.Manager.Site.Service.RenderEngine;

public class LayoutMetadata
{
    [YamlMember(Alias = "layout")]
    public string Layout { get; set; }
}
