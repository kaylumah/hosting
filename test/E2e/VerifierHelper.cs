// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;

namespace Test.E2e
{
    public static partial class VerifierHelper
    {
        //[GeneratedRegex(@"(?<before>https://)(?<val>[a-zA-Z0-9\-\.]*(.net|.nl))(?<after>\/[\w/_-]*\.(html|xml|png|svg))?")]
        [GeneratedRegex(@"(?<before>https://)(?<val>(kaylumah.nl|green-field-0353fee03-[0-9]{3}.westeurope.1.azurestaticapps.net))(?<after>[\w\/\.\-]*)?")]
        public static partial Regex BaseUrl();

        [GeneratedRegex(@"(?<before>>)(?<val>[a-zA-Z0-9 ]*)(?<after> ago<)")]
        public static partial Regex TimeAgo();
    }

    public static class BasePageVerifier
    {
        [ModuleInitializer]
        public static void Init() => VerifyImageHash.Initialize();

        public static async Task VerifyScreenshot(BasePageObject basePageObject)
        {
            byte[] screenshot = await basePageObject.ScreenshotAsync();
            using MemoryStream stream = new MemoryStream(screenshot);
            VerifySettings settings = new VerifySettings();
            await Verifier.Verify(stream, "png", settings);
        }
    }

    public static class HtmlPageVerifier
    {
        public static async Task Verify(HtmlPage page, string methodName = null)
        {
            string html = await page.GetContent();
            Dictionary<string, string> metaTags = await page.GetMetaTags();

            string commitHash = metaTags["kaylumah:commit"];
            string shortCommitHash = commitHash[..7];
            // string version = metaTags["kaylumah:version"];
            string buildId = metaTags["kaylumah:buildId"];
            string buildNumber = metaTags["kaylumah:buildNumber"];

            Regex baseUrlRegex = VerifierHelper.BaseUrl();
            VerifySettings settings = new VerifySettings();
            if (methodName != null)
            {
                settings.UseMethodName(methodName);
            }

            Regex buildNumberRegex = new Regex($"(?<before>(content=\"[0-9.]*|>))(?<val>{buildNumber})(?<after>(\"|<))");

            settings.ScrubMatches(baseUrlRegex, "BaseUrl_");
            settings.ScrubInlineGuids();
            settings.ScrubInlineDateTimeOffsets("yyyy-MM-dd HH:mm:ss zzz");
            settings.AddScrubber(_ => _.Replace(shortCommitHash, "[SHORT-COMMIT-HASH]"));
            settings.AddScrubber(_ => _.Replace(commitHash, "[COMMIT-HASH]"));
            settings.AddScrubber(_ => _.Replace(buildId, "[BUILD-ID]"));
            // settings.AddScrubber(_ => _.Replace(buildNumber, "[BUILD-Number]"));
            // settings.AddScrubber(_ => _.Replace(version, "[BUILD-Version]"));
            settings.ScrubMatches(buildNumberRegex, "BuildNumber_");
            settings.ReplaceMatches(VerifierHelper.TimeAgo(), "Time_Unit");
            await Verifier.Verify(html, "html", settings);
        }
    }
}
