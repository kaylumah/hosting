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
        [Theory]
        [MemberData(nameof(Data))]
        public async Task TestX(string device)
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync();
            var contextOptions = playwright.Devices[device];
            contextOptions.BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL");
            var context = await browser.NewContextAsync(contextOptions);
            var page = await context.NewPageAsync();
            await page.GotoAsync("blog.html");
            var bytes = await page.ScreenshotAsync();
            await File.WriteAllBytesAsync($"Output/{device}.png", bytes);
        }

        public static IEnumerable<object[]> Data()
        {
            List<object[]> result = new List<object[]>();
            result.Add(new object[] {
                "iPhone 14 Pro Max"
            });
            return result;
        }
    }
}