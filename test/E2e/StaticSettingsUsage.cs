// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using VerifyXunit;

namespace Test.E2e
{
    public static class StaticSettingsUsage
    {
        [ModuleInitializer]
        public static void Initialize() => Verifier.UseProjectRelativeDirectory("Snapshots");
    }
}
