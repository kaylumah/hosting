// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Engine.Transformation.Interface;

public class Links
{
    public string Twitter { get;set; }
    public string TwitterProfileUrl => string.IsNullOrEmpty(Twitter) ? null : $"https://twitter.com/{Twitter}";
}
