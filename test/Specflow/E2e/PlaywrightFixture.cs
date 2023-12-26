// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3003
namespace Test.Specflow.E2e
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
            BrowserTypeLaunchOptions options = new BrowserTypeLaunchOptions() {
                Headless = false
            };
            PlaywrightInstance = await Playwright.CreateAsync();
            Browser = await PlaywrightInstance.Chromium.LaunchAsync(options);
        }
    }
}
