// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;

namespace Test.E2e
{

    public static class XmlPageVerifier
    {
        public static async Task Verify(XmlPage page, string methodName = null)
        {
            string xml = await page.GetContent();

            Regex baseUrlRegex = VerifierHelper.BaseUrl();
            VerifySettings settings = new VerifySettings();
            if (methodName != null)
            {
                settings.UseMethodName(methodName);
            }
            settings.ScrubMatches(baseUrlRegex, "BaseUrl_");
            await Verifier.Verify(xml, settings);
        }
    }
}
