// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
#pragma warning disable
namespace Test.E2e.SnapshotTests
{
    public class TempPageHtmlTests : IClassFixture<MobileFixture2>
    {
        readonly MobileFixture2 _MobileFixture;

        public TempPageHtmlTests(MobileFixture2 mobileFixture)
        {
            _MobileFixture = mobileFixture;
        }

        [Fact]
        public async Task Test1()
        {
            IPage page = await _MobileFixture.GetPage();
            BlogPage blogPage = new BlogPage(page);
            await blogPage.NavigateAsync();

            byte[] bytes = await blogPage.ScreenshotAsync();
            await File.WriteAllBytesAsync("Output/Mobile.png", bytes);
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