// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Test.Unit
{
    public class FunctionAppTests
    {
        readonly ILoggerFactory _LoggerFactory;
        readonly UrlFunctions _UrlFunctions;

        public FunctionAppTests()
        {
            _LoggerFactory = new LoggerFactory();
            _UrlFunctions = new UrlFunctions(_LoggerFactory);
        }

        [Fact]
        public void Test_RunWithout_Header()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            HttpRequest request = httpContext.Request;

            Microsoft.AspNetCore.Mvc.IActionResult response = _UrlFunctions.Run(request);
            Microsoft.AspNetCore.Mvc.RedirectResult redirectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectResult>(response);
            Assert.Equal("/404.html", redirectResult.Url);
        }

        [Fact]
        public void Test_RunWithout_RedirectSetup()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            HttpRequest request = httpContext.Request;
            request.Headers["x-ms-original-url"] = "/blog2";

            Microsoft.AspNetCore.Mvc.IActionResult response = _UrlFunctions.Run(request);
            Microsoft.AspNetCore.Mvc.RedirectResult redirectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectResult>(response);
            Assert.Equal("/404.html?originalUrl=/blog2", redirectResult.Url);
        }

        [Fact]
        public void Test_RunWithout_RedirectSetupEndingInHtml()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            HttpRequest request = httpContext.Request;
            request.Headers["x-ms-original-url"] = "/blog2.html";

            Microsoft.AspNetCore.Mvc.IActionResult response = _UrlFunctions.Run(request);
            Microsoft.AspNetCore.Mvc.RedirectResult redirectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectResult>(response);
            Assert.Equal("/404.html?originalUrl=/blog2.html", redirectResult.Url);
        }

        [Fact]
        public void Test_RunWith_RedirectSetup()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            HttpRequest request = httpContext.Request;
            request.Headers["x-ms-original-url"] = "/2023/04/14/csharp-client-for-openapi-revistted.html";

            Microsoft.AspNetCore.Mvc.IActionResult response = _UrlFunctions.Run(request);
            Microsoft.AspNetCore.Mvc.RedirectResult redirectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectResult>(response);
            Assert.Equal("/2023/04/14/csharp-client-for-openapi-revisited.html", redirectResult.Url);
        }

        [Fact]
        public void Test_RunWith_DisabledRedirectSetup()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            HttpRequest request = httpContext.Request;
            request.Headers["x-ms-original-url"] = "/2024/08/06/fix-vscode-markdown-preview.html";

            Microsoft.AspNetCore.Mvc.IActionResult response = _UrlFunctions.Run(request);
            Microsoft.AspNetCore.Mvc.RedirectResult redirectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectResult>(response);
            Assert.Equal("/404.html?originalUrl=/2024/08/06/fix-vscode-markdown-preview.html", redirectResult.Url);
        }
    }
}