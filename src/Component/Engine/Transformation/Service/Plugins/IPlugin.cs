// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Engine.Transformation.Service.Plugins;

public interface IPlugin
{
    string Name { get; }
    string Render(object data);
}