﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Test.E2e
{
    public class ScreenshotFactAttribute : FactAttribute
    {
        public ScreenshotFactAttribute() : base()
        {
            // bool success = bool.TryParse(Environment.GetEnvironmentVariable("CI"), out bool ci);
            bool skipTest = true;// success && ci;
            if (skipTest)
            {
                Skip = "Screenshots are disabled on CI";
            }
        }
    }

    public static partial class VerifierHelper
    {
        //[GeneratedRegex(@"(?<before>https://)(?<val>[a-zA-Z0-9\-\.]*(.net|.nl))(?<after>\/[\w/_-]*\.(html|xml|png|svg))?")]
        [GeneratedRegex(@"(?<before>https://)(?<val>(kaylumah.nl|green-field-0353fee03-[0-9]+.westeurope.1.azurestaticapps.net))(?<after>[\w\/\.\-]*)?")]
        public static partial Regex BaseUrl();

        [GeneratedRegex(@"(?<before>>)(?<val>[a-zA-Z0-9 -]*)(?<after> ago<)")]
        public static partial Regex TimeAgo();

        [GeneratedRegex(@"(?<before>var words = \[)(?<val>(.|\n)*)(?<after>];)")]
        public static partial Regex TagCloud();
    }

    public static class BasePageVerifier
    {
        [ModuleInitializer]
        public static void Init() => VerifyImageHash.Initialize();

        public static async Task VerifyScreenshot(BasePageObject basePageObject)
        {
            byte[] screenshot = await basePageObject.ScreenshotAsync();
            using MemoryStream stream = new MemoryStream(screenshot);
            VerifySettings settings = new VerifySettings();
            await Verifier.Verify(stream, "png", settings);
        }
    }

    public static class HtmlPageVerifier
    {
        public static async Task VerifyHead(HtmlPage page, string? methodName = null)
        {
            string? html = await page.GetHead() ?? string.Empty;
            html = html.Replace("/Users/maxhamulyak/", "/ExamplePath/");
            Dictionary<string, string?> metaTags = await page.GetMetaTags();

            string? commitHash = metaTags["kaylumah:commit"];
            string shortCommitHash = string.IsNullOrEmpty(commitHash) ? string.Empty : commitHash[..7];
            // string version = metaTags["kaylumah:version"];
            string? buildId = metaTags["kaylumah:buildId"];
            string? buildNumber = metaTags["kaylumah:buildNumber"];

            Regex baseUrlRegex = VerifierHelper.BaseUrl();
            VerifySettings settings = new VerifySettings();
            if (methodName != null)
            {
                // settings.UseMethodName(methodName);
            }

            Regex buildNumberRegex = new Regex($"(?<before>(content=\"[0-9.]*|>))(?<val>{buildNumber})(?<after>(\"|<))");

            settings.ReplaceMatches(baseUrlRegex, "BaseUrl_1");
            settings.ScrubInlineGuids();
            settings.ScrubInlineDateTimeOffsets("yyyy-MM-dd HH:mm:ss zzz");
            settings.AddScrubber(_ => _.Replace(shortCommitHash, "[SHORT-COMMIT-HASH]"));
            if (commitHash != null)
            {
                settings.AddScrubber(_ => _.Replace(commitHash, "[COMMIT-HASH]"));
            }

            if (buildId != null)
            {
                settings.AddScrubber(_ => _.Replace(buildId, "[BUILD-ID]"));
            }
            // settings.AddScrubber(_ => _.Replace(buildNumber, "[BUILD-Number]"));
            // settings.AddScrubber(_ => _.Replace(version, "[BUILD-Version]"));
            settings.ScrubMatches(buildNumberRegex, "BuildNumber_");
#pragma warning disable IDESIGN103
            settings.ReplaceMatches(VerifierHelper.TimeAgo(), "Time_Unit");
            settings.ReplaceMatches(VerifierHelper.TagCloud(), string.Empty);
            await Verifier.Verify(html, "html", settings);
        }
        
        public static async Task Verify(HtmlPage page, string? methodName = null)
        {
            string? html = await page.GetContent() ?? string.Empty;
            html = html.Replace("/Users/maxhamulyak/", "/ExamplePath/");
            Dictionary<string, string?> metaTags = await page.GetMetaTags();

            string? commitHash = metaTags["kaylumah:commit"];
            string shortCommitHash = string.IsNullOrEmpty(commitHash) ? string.Empty : commitHash[..7];
            // string version = metaTags["kaylumah:version"];
            string? buildId = metaTags["kaylumah:buildId"];
            string? buildNumber = metaTags["kaylumah:buildNumber"];

            Regex baseUrlRegex = VerifierHelper.BaseUrl();
            VerifySettings settings = new VerifySettings();
            if (methodName != null)
            {
                // settings.UseMethodName(methodName);
            }

            Regex buildNumberRegex = new Regex($"(?<before>(content=\"[0-9.]*|>))(?<val>{buildNumber})(?<after>(\"|<))");

            settings.ReplaceMatches(baseUrlRegex, "BaseUrl_1");
            settings.ScrubInlineGuids();
            settings.ScrubInlineDateTimeOffsets("yyyy-MM-dd HH:mm:ss zzz");
            settings.AddScrubber(_ => _.Replace(shortCommitHash, "[SHORT-COMMIT-HASH]"));
            if (commitHash != null)
            {
                settings.AddScrubber(_ => _.Replace(commitHash, "[COMMIT-HASH]"));
            }

            if (buildId != null)
            {
                settings.AddScrubber(_ => _.Replace(buildId, "[BUILD-ID]"));
            }
            // settings.AddScrubber(_ => _.Replace(buildNumber, "[BUILD-Number]"));
            // settings.AddScrubber(_ => _.Replace(version, "[BUILD-Version]"));
            settings.ScrubMatches(buildNumberRegex, "BuildNumber_");
#pragma warning disable IDESIGN103
            settings.ReplaceMatches(VerifierHelper.TimeAgo(), "Time_Unit");
            settings.ReplaceMatches(VerifierHelper.TagCloud(), string.Empty);
            await Verifier.Verify(html, "html", settings);
        }
    }
}
