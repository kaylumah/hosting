// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class LdJsonTargetAttribute : Attribute
    {
        public Type TargetType
        { get; }

        public LdJsonTargetAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}