// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Scriban;
using Scriban.Runtime;

namespace Kaylumah.Ssg.Utilities
{
    public class SeoPlugin : IPlugin
    {
        private readonly string _raw;
        public string Name => "seo";

        public SeoPlugin()
        {
            _raw = System.IO.File.ReadAllText("seo_template.html");
        }

        public string Render(object data)
        {
            var liquidTemplate = Template.ParseLiquid(_raw);
            var context = new LiquidTemplateContext();
            var ld = LdJson(data);
            var scriptObject = new ScriptObject
            {
                ["seo_tag"] = data,
                ["json_ld"] = ld
            };
            scriptObject.Import(typeof(GlobalFunctions));
            context.PushGlobal(scriptObject);
            var pluginResult = liquidTemplate.Render(context);
            return pluginResult;
        }

        private Dictionary<string, object> LdJson(object o)
        {
            return new Dictionary<string, object>()
            {
                { "datePublished", DateTime.Now.ToShortDateString() },
                { "dateModified", DateTime.Now.ToShortDateString() },
                { "@type", "BlogPosting" },
                { "url", "https://kaylumah.nl/..." },
                { "image", "https://kaylumah.nl/assets/images/social_preview.png" },
                {
                    "publisher",
                    new Dictionary<string, object>()
                    {
                        { "@type", "Organization" },
                        {
                            "logo",
                            new Dictionary<string, object>()
                            {
                                { "@type", "ImageObject" },
                                { "url", "https://kaylumah.nl/assets/logo.svg" }
                            }
                        },
                        { "name", "max" }
                    }
                },
                { "mainEntityOfPage", new Dictionary<string, object>()
                    {
                        { "@type", "WebPage" },
                        { "@id", "https://kaylumah.nl/2020/08/01/kaylumah-the-new-home-for-blogs-written-by-max-hamulyak.html" }
                    }
                },
                {
                    "author",
                    new Dictionary<string, object>()
                    {
                        { "@type", "Person" },
                        { "name", "max" }
                    }
                },
                { "headline", "TODO" },
                { "description", "TODO" },
                { "@context", "https://schema.org" }
            };
        }
    }
}