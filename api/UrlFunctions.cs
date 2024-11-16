// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Api
{

    public class UrlFunctions
    {
        readonly ILogger _Logger;
        readonly List<RedirectOption> _RedirectOptions;

        public UrlFunctions(ILoggerFactory loggerFactory)
        {
            _Logger = loggerFactory.CreateLogger<UrlFunctions>();
            _RedirectOptions = new List<RedirectOption>();

            RedirectOption fixTypo = new RedirectOption("/2023/04/14/csharp-client-for-openapi-revistted.html", "/2023/04/14/csharp-client-for-openapi-revisited.html");
            fixTypo.Enabled = true;
            fixTypo.Permanent = true;
            _RedirectOptions.Add(fixTypo);

            RedirectOption rewritePreType = new RedirectOption(
                @"^\/(?<year>\d{4})\/(?<month>\d{2})\/(?<day>\d{2})\/(?<rest>[\w-]*?)(?<ext>\.\w+)?$",
                "/articles/${year}/${month}/${day}/${rest}.html");
            rewritePreType.Enabled = false;
            rewritePreType.Permanent = true;
            _RedirectOptions.Add(rewritePreType);

        }

        [Function("fallback")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            IActionResult result;
            bool hasHeader = req.Headers.TryGetValue("x-ms-original-url", out Microsoft.Extensions.Primitives.StringValues headerValue);
            if (hasHeader)
            {
                string originalUrl = headerValue.ToString();
                Uri uri = new Uri(originalUrl);
                string path = uri.AbsolutePath;
                _Logger.LogInformation("Original Url: {OriginalUrl}", originalUrl);

                RedirectOption? option = _RedirectOptions.FirstOrDefault(o => o.Enabled && Regex.IsMatch(path, o.Pattern));
                if (option != null)
                {
                    string newPath = Regex.Replace(path, option.Pattern, option.Rewrite);
                    result = new RedirectResult(newPath, option.Permanent);
                }
                else
                {
                    result = new RedirectResult($"/404.html?originalUrl={path}");
                }
            }
            else
            {
                result = new RedirectResult("/404.html");
            }

            return result;
        }
    }
}