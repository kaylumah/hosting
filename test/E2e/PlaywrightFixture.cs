// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3002 // Return type is not CLS-compliant
namespace Test.E2e
{
    public class PlaywrightFixture : IAsyncLifetime
    {
        IPlaywright PlaywrightInstance { get; set; }
        IBrowser Browser { get; set; }

        public async Task DisposeAsync()
        {
            if (Browser != null)
            {
                await Browser.DisposeAsync();
            }

            PlaywrightInstance.Dispose();
        }

        public async Task InitializeAsync()
        {
            PlaywrightInstance = await Playwright.CreateAsync();
            await GetBrowser();
        }

        public async Task<IBrowser> GetBrowser()
        {
            if (Browser == null)
            {
                Browser = await PlaywrightInstance.Chromium.LaunchAsync();
            }

            return Browser;
        }

        public async Task<IPage> GetPage()
        {
            BrowserNewContextOptions contextOptions = ContextOptions();
            IBrowserContext context = await Browser.NewContextAsync(contextOptions);
            IPage page = await context.NewPageAsync();
            return page;
        }

        public BrowserNewContextOptions ContextOptions()
        {
            BrowserNewContextOptions browserNewContextOptions = new BrowserNewContextOptions();
            browserNewContextOptions.BaseURL = GetBaseUrl();
            return browserNewContextOptions;
        }

        public string GetBaseUrl()
        {
            string result = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
            return result;
        }
    }
}
