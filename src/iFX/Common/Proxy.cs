// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using Castle.DynamicProxy;

namespace Kaylumah.Ssg.Utilities.Common
{
    public static class Proxy
    {
        public static readonly ProxyGenerator ProxyGenerator;

        static Proxy()
        {
            ProxyGenerator = new();
        }
    }
}