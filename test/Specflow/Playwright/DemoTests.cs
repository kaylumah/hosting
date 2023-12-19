// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3003
namespace Test.Specflow.Playwright
{
    public class PlaywrightFixture : IAsyncLifetime
    {
        public IBrowser Browser { get; set; } = null!;
        private IPlaywright PlaywrightInstance { get; set; } = null!;

        public async Task DisposeAsync()
        {
            await Browser.DisposeAsync();
            PlaywrightInstance.Dispose();
        }

        public async Task InitializeAsync()
        {
            PlaywrightInstance = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await PlaywrightInstance.Chromium.LaunchAsync();
        }
    }

    [CollectionDefinition(nameof(PlaywrightFixture))]
    public class PlaywrightCollection : ICollectionFixture<PlaywrightFixture>
    {}

    [Collection(nameof(PlaywrightFixture))]
    public class PlaywrightTests
    {
        private readonly IBrowser _browser;

        public PlaywrightTests(PlaywrightFixture playwrightFixture)
        {
            _browser = playwrightFixture.Browser;
        }

        [Fact]
        public async Task Test1()
        {
            IPage page = await _browser.NewPageAsync();
            await page.GotoAsync("https://google.com");
            ILocator locator = page.Locator("input#gbqfbb");
            int actual = await locator.CountAsync();
            Assert.Equal(1, actual);
        }
    }
}
