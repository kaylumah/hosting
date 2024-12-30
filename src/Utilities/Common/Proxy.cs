// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Castle.DynamicProxy;

namespace Microsoft.Extensions.DependencyInjection
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