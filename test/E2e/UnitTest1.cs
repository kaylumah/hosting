// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.E2e
{
    [TestClass]
#pragma warning disable CS3009 // Base type is not CLS-compliant
    public class UnitTest1 : PlaywrightTest
#pragma warning restore CS3009 // Base type is not CLS-compliant
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            IAPIRequestContext context = await Playwright.APIRequest.NewContextAsync();
            IAPIResponse response = await context.GetAsync("feed.xml");
            await Expect(response).ToBeOKAsync();
        }
    }
}
