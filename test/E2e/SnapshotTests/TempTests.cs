// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
#pragma warning disable
namespace Test.E2e.SnapshotTests
{
    public class TempPageHtmlTests :
        IClassFixture<MobileBreakpoint>,
        IClassFixture<TabletBreakpoint>,
        IClassFixture<LaptopBreakpoint>,
        IClassFixture<DesktopBreakpoint>
    {
        readonly MobileBreakpoint _MobileFixture;
        readonly TabletBreakpoint _TabletFixture;
        readonly LaptopBreakpoint _LaptopFixture;
        readonly DesktopBreakpoint _DesktopFixture;

        public TempPageHtmlTests(MobileBreakpoint mobileFixture,
            TabletBreakpoint tabletFixture,
            LaptopBreakpoint laptopFixture,
            DesktopBreakpoint desktopFixture)
        {
            _MobileFixture = mobileFixture;
            _TabletFixture = tabletFixture;
            _LaptopFixture = laptopFixture;
            _DesktopFixture = desktopFixture;
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

        [Fact]
        public async Task Test3()
        {
            IPage page = await _LaptopFixture.GetPage();
            BlogPage blogPage = new BlogPage(page);
            await blogPage.NavigateAsync();

            byte[] bytes = await blogPage.ScreenshotAsync();
            await File.WriteAllBytesAsync("Output/Laptop.png", bytes);
        }

        [Fact]
        public async Task Test4()
        {
            IPage page = await _DesktopFixture.GetPage();
            BlogPage blogPage = new BlogPage(page);
            await blogPage.NavigateAsync();

            byte[] bytes = await blogPage.ScreenshotAsync();
            await File.WriteAllBytesAsync("Output/Desktop.png", bytes);
        }
    }

    public class MobileBreakpoint : PlaywrightFixture
    {
        protected override BrowserNewContextOptions CreateBrowserNewContextOptions()
        {
            var devices = PlaywrightInstance.Devices;
            BrowserNewContextOptions result = new BrowserNewContextOptions();
            result.ScreenSize = new ScreenSize()
            {
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
            result.ScreenSize = new ScreenSize()
            {
                Width = 1024 - 1,
                Height = 1200
            };
            return result;
        }
    }

    public class LaptopBreakpoint : PlaywrightFixture
    {
        protected override BrowserNewContextOptions CreateBrowserNewContextOptions()
        {
            var devices = PlaywrightInstance.Devices;
            BrowserNewContextOptions result = new BrowserNewContextOptions();
            result.ScreenSize = new ScreenSize()
            {
                Width = 1280 - 1,
                Height = 1200
            };
            return result;
        }
    }

    public class DesktopBreakpoint : PlaywrightFixture
    {
        protected override BrowserNewContextOptions CreateBrowserNewContextOptions()
        {
            var devices = PlaywrightInstance.Devices;
            BrowserNewContextOptions result = new BrowserNewContextOptions();
            result.ScreenSize = new ScreenSize()
            {
                Width = 1280 + 1,
                Height = 1200
            };
            return result;
        }
    }
}