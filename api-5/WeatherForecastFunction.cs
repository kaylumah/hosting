// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class HttpTrigger
    {
        private readonly ILogger _logger;

        public HttpTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger>();
        }

        [Function("fallback")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            bool hasHeader = req.Headers.TryGetValue("x-ms-original-url", out  Microsoft.Extensions.Primitives.StringValues originalUrl);

            _logger.LogInformation($"Original Url: {originalUrl}");

            Uri? uri = new Uri(originalUrl.ToString());
            _logger.LogInformation(uri.AbsolutePath);

            /*
                        string AbsolutePath = "/2024/011/02/as-s.html";
            // string AbsolutePath = "blog.html";
            string pattern = @"^/(?<year>2024)(?<rest>.*)";
            string replacement = "/something/${year}${rest}";

            if (Regex.IsMatch(AbsolutePath, pattern))
            {
                string newUrl = Regex.Replace(AbsolutePath, pattern, replacement);
            }*/


            RedirectResult result = new RedirectResult($"/404?originalUrl={uri.AbsolutePath}");
            result.Permanent = true;
            return result;

            // Fallback to 404 (does not work)
            // NotFoundResult result = new NotFoundResult();
            // return result;
        }
    }
}