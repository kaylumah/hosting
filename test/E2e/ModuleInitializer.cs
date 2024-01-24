// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using AngleSharp.Diffing;
using VerifyTests;
using VerifyTests.AngleSharp;

#pragma warning disable CS3002 // Return type is not CLS-compliant
namespace Test.E2e
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifyAngleSharpDiffing.Initialize(action =>
            {
                AngleSharp.Diffing.Strategies.IDiffingStrategyCollection options = action.AddDefaultOptions();
            });
            // HtmlPrettyPrint.All();
        }
    }
}
