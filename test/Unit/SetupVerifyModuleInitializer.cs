// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace Test.Unit
{
    public static class SetupVerifyModuleInitializer
    {
        [ModuleInitializer]
        public static void InitVerify()
        {
            // Console.WriteLine("[Verify] Module initializer started.");

            // Set shared / static settings for Verify in this file
        }
    }
}