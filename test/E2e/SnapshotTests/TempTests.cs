// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.Playwright;
using Xunit;

namespace Test.E2e.SnapshotTests
{
    public class TempPageHtmlTests : IClassFixture<MobileFixture2>
    {
        readonly MobileFixture2 _MobileFixture;

        public TempPageHtmlTests(MobileFixture2 mobileFixture)
        {
            _MobileFixture = mobileFixture;
        }

        public void Test1()
        {
            _MobileFixture.
        }
    }

    public class MobileFixture2 : PlaywrightFixture
    {
        protected override BrowserNewContextOptions CreateBrowserNewContextOptions()
        {
            BrowserNewContextOptions result = new BrowserNewContextOptions();
            return result;
        }
    }
}