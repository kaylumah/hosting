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
    public class RedirectOption
    {
        public bool Enabled
        { get; set; }
        public bool Permanent
        { get; set; }
        public string Pattern
        { get; set; }
        public string Rewrite
        { get; set; }

        public RedirectOption(string pattern, string rewrite)
        {
            Pattern = pattern;
            Rewrite = rewrite;
        }
    }

    public class HttpTrigger
    {
        readonly ILogger _Logger;
        readonly List<RedirectOption> _RedirectOptions;

        public HttpTrigger(ILoggerFactory loggerFactory)
        {
            _Logger = loggerFactory.CreateLogger<HttpTrigger>();
            _RedirectOptions = new List<RedirectOption>();
            RedirectOption option = new RedirectOption(
                @"\/(?<year>\d{4})\/(?<month>\d{2})\/(?<day>\d{2})\/(?<rest>[\w-]*?)(?<ext>\.\w+)?$",
                "/something/${year}/${rest}.html");
            option.Enabled = true;
            option.Permanent = true;
            _RedirectOptions.Add(option);
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