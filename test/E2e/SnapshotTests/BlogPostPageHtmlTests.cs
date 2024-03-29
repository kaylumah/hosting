﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

#pragma warning disable CS3016
namespace Test.E2e.SnapshotTests
{
    public class BlogPostPageHtmlTests : IClassFixture<DesktopFixture>, IClassFixture<MobileFixture>
    {
        readonly DesktopFixture _DesktopFixture;
        readonly MobileFixture _MobileFixture;

        public BlogPostPageHtmlTests(DesktopFixture desktopFixture, MobileFixture mobileFixture)
        {
            _DesktopFixture = desktopFixture;
            _MobileFixture = mobileFixture;
        }

        [Theory]
        [MemberData(nameof(GetBlogPages))]
        public async Task Verify_BlogPostPageHtml_Contents(string path)
        {
            IPage blogPage = await _DesktopFixture.GetPage();
            BlogItemPage blogItemPage = new BlogItemPage(path, blogPage);
            await blogItemPage.NavigateAsync();
            string testParameter = path
                .Replace("/", "_")
                .Replace(".html", "");
            string methodName = $"{nameof(Verify_BlogPostPageHtml_Contents)}_{testParameter}";
            await HtmlPageVerifier.Verify(blogItemPage, methodName);
        }

        public static IEnumerable<object[]> GetBlogPages()
        {
            yield return new object[] { "2024/02/09/long-live-reqnroll.html" };
            yield return new object[] { "2023/04/14/csharp-client-for-openapi-revistted.html" };
            yield return new object[] { "2022/09/17/how-to-use-azurite-for-testing-azure-storage-in-dotnet.html" };
            yield return new object[] { "2022/06/07/share-debug-configuration-with-launch-profiles.html" };
            yield return new object[] { "2022/02/21/working-with-azure-sdk-for-dotnet.html" };
            yield return new object[] { "2022/01/31/improve-code-quality-with-bannedsymbolanalyzers.html" };
            yield return new object[] { "2021/11/29/validated-strongly-typed-ioptions.html" };
            yield return new object[] { "2021/11/14/capture-logs-in-unit-tests.html" };
            yield return new object[] { "2021/07/17/decreasing-solution-build-time-with-filters.html" };
            yield return new object[] { "2021/05/23/generate-csharp-client-for-openapi.html" };
            yield return new object[] { "2021/04/11/an-approach-to-writing-mocks.html" };
            yield return new object[] { "2021/03/27/set-nuget-metadata-via-msbuild.html" };
            yield return new object[] { "2019/09/07/using-csharp-code-your-git-hooks.html" };
        }
    }
}
