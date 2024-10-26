// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
#pragma warning disable
namespace Test.E2e.SnapshotTests
{
    public class TempPageHtmlTests
    {
        [Theory(Skip = "used to test layout changes!")]
        [MemberData(nameof(Data))]
        public async Task TestX(string device)
        {
            IPlaywright playwright = await Playwright.CreateAsync();
            IBrowser browser = await playwright.Chromium.LaunchAsync();
            BrowserNewContextOptions contextOptions = playwright.Devices[device];
            contextOptions.BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL");
            IBrowserContext context = await browser.NewContextAsync(contextOptions);
            IPage page = await context.NewPageAsync();
            await page.GotoAsync("blog.html");
            byte[] bytes = await page.ScreenshotAsync();
            await File.WriteAllBytesAsync($"Output/{device}.png", bytes);
        }

        public static IEnumerable<object[]> Data()
        {
            List<object[]> result = new List<object[]>();
            result.Add(new object[] { "iPad Mini" });
            result.Add(new object[] { "iPad (gen 7)" });
            result.Add(new object[] { "iPad Pro 11" });
            result.Add(new object[] { "iPhone 14" });
            result.Add(new object[] { "iPhone 14 Plus" });
            result.Add(new object[] { "iPhone 14 Pro" });
            result.Add(new object[] { "iPhone 14 Pro Max" });
            result.Add(new object[] { "Desktop Chrome" });
            result.Add(new object[] { "Desktop Chrome HiDPI" });
            return result;
        }
    }
}