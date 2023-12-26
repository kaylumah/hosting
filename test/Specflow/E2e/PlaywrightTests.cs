// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3003
namespace Test.Specflow.E2e
{

    [Collection(nameof(PlaywrightFixture))]
    public class PlaywrightTests
    {
        readonly IBrowser _Browser;

        public PlaywrightTests(PlaywrightFixture playwrightFixture)
        {
            _Browser = playwrightFixture.Browser;
        }

        [Fact]
        public async Task Test1()
        {
            // IPage page = await _browser.NewPageAsync();
            // await page.GotoAsync("https://google.com");
            // ILocator locator = page.Locator("input#gbqfbb");
            // int actual = await locator.CountAsync();
            // Assert.Equal(1, actual);
        }

        [Fact]
        public async Task Test2()
        {
            await using IBrowserContext context =  await _Browser.NewContextAsync();

            await context.Tracing.StartAsync(new() {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            IPage page = await context.NewPageAsync();
            await page.GotoAsync("https://playwright.dev");

            // Stop tracing and export it into a zip archive.
            await context.Tracing.StopAsync(new()
            {
                Path = "trace.zip"
            });
        }
    }
}
