// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using Scriban;
using Scriban.Runtime;

namespace Kaylumah.Ssg.Utilities
{
    public class FeedPlugin : IPlugin
    {
        private readonly string _raw;
        public string Name => "feed_meta";
        public FeedPlugin()
        {
            _raw = System.IO.File.ReadAllText("feed_template.html");
        }

        public string Render(object data)
        {
            var liquidTemplate = Template.ParseLiquid(_raw);
            var context = new LiquidTemplateContext();
            // var ld = LdJson(data);
            var scriptObject = new ScriptObject();
            // {
            //     ["seo_tag"] = data,
            //     ["json_ld"] = ld
            // };
            scriptObject.Import(typeof(GlobalFunctions));
            context.PushGlobal(scriptObject);
            var pluginResult = liquidTemplate.Render(context);
            return pluginResult;
        }
    }
}