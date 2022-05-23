// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

[DebuggerDisplay("TagMetaData '{Name}'")]
public class TagMetaData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}
