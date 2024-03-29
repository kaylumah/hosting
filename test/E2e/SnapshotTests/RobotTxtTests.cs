﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Test.E2e.SnapshotTests
{
    public class RobotTxtTests : IClassFixture<DesktopFixture>
    {
        readonly DesktopFixture _DesktopFixture;

        public RobotTxtTests(DesktopFixture desktopFixture)
        {
            _DesktopFixture = desktopFixture;
        }

        [Fact]
        public async Task Verify_RobotTxt_Contents()
        {
            IPage page = await _DesktopFixture.GetPage();
            RobotsPage robotsPage = new RobotsPage(page);
            await robotsPage.NavigateAsync();
            page.Url.Should().EndWith(robotsPage.PagePath);

            string txt = await robotsPage.GetContent();
            VerifySettings settings = new VerifySettings();
            Regex regex = VerifierHelper.BaseUrl();
            settings.ScrubMatches(regex, "BaseUrl_");
            await Verifier.Verify(txt, settings);
        }
    }
}
