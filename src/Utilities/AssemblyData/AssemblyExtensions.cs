// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kaylumah.Ssg.Utilities;

public static class AssemblyExtensions
{
    public static T GetAttribute<T>(this Assembly assembly)
    {
        return assembly.GetCustomAttributes(typeof(T)).Cast<T>().Single();
    }

    public static IEnumerable<T> GetAttribtutes<T>(this Assembly assembly)
    {
        return assembly.GetCustomAttributes(typeof(T))
            .Cast<T>();
    }
}