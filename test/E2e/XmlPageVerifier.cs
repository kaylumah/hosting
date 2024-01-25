// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using VerifyTests;

namespace Test.E2e
{
    public static partial class XmlPageVerifier
    {
        public static async Task Verify(XmlPage page, string methodName = null)
        {
            string xml = await page.GetContent();

            VerifySettings settings = new VerifySettings();
            if (methodName != null)
            {
                settings.UseMethodName(methodName);
            }
        }
    }
}
