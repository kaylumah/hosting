// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using static Microsoft.Playwright.Assertions;

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
        public async Task Test_HomePage()
        {
            // IPage page = await _browser.NewPageAsync();
            // await page.GotoAsync("https://google.com");
            // ILocator locator = page.Locator("input#gbqfbb");
            // int actual = await locator.CountAsync();
            // Assert.Equal(1, actual);

            IBrowserContext context = await _Browser.NewContextAsync();
            IPage page = await context.NewPageAsync();
            await page.GotoAsync("https://kaylumah.nl/");

            // await Expect(Page).ToHaveURLAsync("https://playwright.dev/");
            
            await Expect(page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new() { Name = "Kaylumah" })).ToBeVisibleAsync();
            await Expect(page.GetByRole(AriaRole.Link, new() { Name = "About" })).ToBeVisibleAsync();
            await Expect(page.GetByRole(AriaRole.Link, new() { Name = "Blog" })).ToBeVisibleAsync();
        }
    }
}
