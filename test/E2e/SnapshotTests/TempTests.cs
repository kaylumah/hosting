// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
#pragma warning disable
namespace Test.E2e.SnapshotTests
{
    public class TempPageHtmlTests : IClassFixture<MobileBreakpoint>, IClassFixture<TabletBreakpoint>
    {
        readonly MobileBreakpoint _MobileFixture;
        readonly TabletBreakpoint _TabletFixture;

        public TempPageHtmlTests(MobileBreakpoint mobileFixture, TabletBreakpoint tabletFixture)
        {
            _MobileFixture = mobileFixture;
            _TabletFixture = tabletFixture;
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

        [Fact]
        public async Task Test2()
        {
            IPage page = await _TabletFixture.GetPage();
            BlogPage blogPage = new BlogPage(page);
            await blogPage.NavigateAsync();

            byte[] bytes = await blogPage.ScreenshotAsync();
            await File.WriteAllBytesAsync("Output/Tablet.png", bytes);
        }
    }

    public class MobileBreakpoint : PlaywrightFixture
    {
        protected override BrowserNewContextOptions CreateBrowserNewContextOptions()
        {
            var devices = PlaywrightInstance.Devices;
            BrowserNewContextOptions result = new BrowserNewContextOptions();
            result.ScreenSize = new ScreenSize() { 
                Width = 640 - 1,
                Height = 1200
            };
            return result;
        }
    }

    public class TabletBreakpoint : PlaywrightFixture
    {
        protected override BrowserNewContextOptions CreateBrowserNewContextOptions()
        {
            var devices = PlaywrightInstance.Devices;
            BrowserNewContextOptions result = new BrowserNewContextOptions();
            result.ScreenSize = new ScreenSize() { 
                Width = 1024 - 1,
                Height = 1200
            };
            return result;
        }
    }
}