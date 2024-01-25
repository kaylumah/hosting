// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;

namespace Test.E2e
{
    public static partial class HtmlPageVerifier
    {
        [GeneratedRegex(@"(?<before>https://)(?<val>[a-zA-Z0-9\-\.]*(.net|.nl))(?<after>\/[a-zA-Z/_]*\.(html|xml|png))")]
        private static partial Regex BaseUrl();

        public static async Task Verify(HtmlPage page)
        {
            string html = await page.GetContent();
            Dictionary<string, string> metaTags = await page.GetMetaTags();

            string commitHash = metaTags["kaylumah:commit"];
            string shortCommitHash = commitHash[..7];
            string buildId = metaTags["kaylumah:buildId"];
            string buildNumber = metaTags["kaylumah:buildNumber"];

            Regex baseUrlRegex = BaseUrl();
            VerifySettings settings = new VerifySettings();
            settings.ScrubMatches(baseUrlRegex);
            settings.ScrubInlineGuids();
            settings.ScrubInlineDateTimeOffsets("yyyy-MM-dd HH:mm:ss zzz");
            settings.ScrubInlineDateTimeOffsets("MM/dd/yyyy HH:mm:ss zzz");
            settings.AddScrubber(_ => _.Replace(shortCommitHash, "short_hash"));
            settings.AddScrubber(_ => _.Replace(commitHash, "longhash"));
            settings.AddScrubber(_ => _.Replace(buildId, "buildId"));
            settings.AddScrubber(_ => _.Replace(buildNumber, "buildNumber"));

            await Verifier.Verify(html, "html", settings);
        }
    }
}
