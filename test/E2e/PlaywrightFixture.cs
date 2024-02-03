// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3001 // Argument type is not CLS-compliant
#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3003 // Type is not CLS-compliant
namespace Test.E2e
{
    public abstract class PlaywrightFixture : IAsyncLifetime
    {
        protected IPlaywright PlaywrightInstance { get; set; }
        protected IBrowser Browser { get; set; }

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
            BrowserNewContextOptions contextOptions = GetBrowserNewContextOptions();
            IBrowserContext context = await Browser.NewContextAsync(contextOptions);
            IPage page = await context.NewPageAsync();
            return page;
        }

        public BrowserNewContextOptions GetBrowserNewContextOptions()
        {
            BrowserNewContextOptions browserNewContextOptions = CreateBrowserNewContextOptions();
            ApplyDefaults(browserNewContextOptions);
            return browserNewContextOptions;
        }

        protected virtual void ApplyDefaults(BrowserNewContextOptions browserNewContextOptions)
        {
            browserNewContextOptions.BaseURL = GetBaseUrl();
        }

        protected abstract BrowserNewContextOptions CreateBrowserNewContextOptions();

        public string GetBaseUrl()
        {
            string result = Environment.GetEnvironmentVariable("PLAYWRIGHT_TEST_BASE_URL") ?? "https://kaylumah.nl";
            return result;
        }
    }

    public class DesktopFixture : PlaywrightFixture
    {
        protected override BrowserNewContextOptions CreateBrowserNewContextOptions()
        {
            const string target = "Desktop Chrome";
            BrowserNewContextOptions options = PlaywrightInstance.Devices[target];
            return options;
        }
    }

    public class MobileFixture : PlaywrightFixture
    {
        protected override BrowserNewContextOptions CreateBrowserNewContextOptions()
        {
            const string target = "iPhone 14 Pro Max";
            BrowserNewContextOptions options = PlaywrightInstance.Devices[target];
            return options;
        }
    }
}
