﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using YamlDotNet.Serialization;

namespace Kaylumah.Ssg.Engine.Transformation.Service;

public class LayoutMetadata
{
    [YamlMember(Alias = "layout")]
    public string Layout { get; set; }
}