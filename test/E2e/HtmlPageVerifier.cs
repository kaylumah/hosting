// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;

namespace Test.E2e
{

    public static class HtmlPageVerifier
    {
        public static async Task Verify(HtmlPage page, string methodName = null)
        {
            string html = await page.GetContent();
            Dictionary<string, string> metaTags = await page.GetMetaTags();

            string commitHash = metaTags["kaylumah:commit"];
            string shortCommitHash = commitHash[..7];
            string buildId = metaTags["kaylumah:buildId"];
            string buildNumber = metaTags["kaylumah:buildNumber"];

            Regex baseUrlRegex = VerifierHelper.BaseUrl();
            VerifySettings settings = new VerifySettings();
            if (methodName != null)
            {
                settings.UseMethodName(methodName);
            }

            settings.ScrubMatches(baseUrlRegex, "BaseUrl_");
            settings.ScrubInlineGuids();
            settings.ScrubInlineDateTimeOffsets("yyyy-MM-dd HH:mm:ss zzz");
            settings.ScrubInlineDateTimeOffsets("MM/dd/yyyy HH:mm:ss zzz");
            settings.AddScrubber(_ => _.Replace(shortCommitHash, "[SHORT-COMMIT-HASH]"));
            settings.AddScrubber(_ => _.Replace(commitHash, "[COMMIT-HASH]"));
            settings.AddScrubber(_ => _.Replace(buildId, "[BUILD-ID]"));
            settings.AddScrubber(_ => _.Replace(buildNumber, "[BUILD-Number]"));
            await Verifier.Verify(html, "html", settings);
        }
    }
}
