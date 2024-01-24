// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AngleSharp.Diffing;
using VerifyTests;
using VerifyTests.AngleSharp;

#pragma warning disable CS3002 // Return type is not CLS-compliant
namespace Test.E2e
{
    public static class ModuleInitializer
    {
        // [ModuleInitializer]
        // public static void Initialize() =>
        //     VerifierSettings.AddScrubber(_ => _.Replace("String to verify", "new value"));

         [ModuleInitializer]
        public static void Initialize()
        {
            VerifierSettings.AddScrubber(_ => _.Replace("String to verify", "new value"));
            Regex regex = new Regex(@"(?<before>\?v=)(?<val>[a-zA-Z0-9]{7})");
            VerifierSettings.ScrubMatches(regex);
        }

        [ModuleInitializer]
        public static void Init()
        {


            
            // VerifyAngleSharpDiffing.Initialize(action =>
            // {
            //     AngleSharp.Diffing.Strategies.IDiffingStrategyCollection options = action.AddDefaultOptions();
            // });
            // HtmlPrettyPrint.All(nodes => {
            //     nodes.ScrubAttributes(x => {
            //         string ownerElementName = x.OwnerElement.LocalName;
            //         if ("html".Equals(ownerElementName))
            //         {

            //         }
            //         else if ("meta".Equals(ownerElementName))
            //         {

            //         }
            //         else if ("link".Equals(ownerElementName))
            //         {

            //         }
            //         else if ("script".Equals(ownerElementName))
            //         {

            //         }
            //         else if ("body".Equals(ownerElementName))
            //         {

            //         }
            //         else if ("div".Equals(ownerElementName))
            //         {

            //         }
            //         else if ("header".Equals(ownerElementName))
            //         {

            //         }
            //         else if ("nav".Equals(ownerElementName))
            //         {

            //         }
            //         else
            //         {

            //         }

            //         return null;
            //     });
            // });
        }
    }
}
