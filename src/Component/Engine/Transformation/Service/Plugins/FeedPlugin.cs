// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Reflection;
using Kaylumah.Ssg.Utilities;
using Scriban;
using Scriban.Runtime;

namespace Kaylumah.Ssg.Engine.Transformation.Service.Plugins;

public class FeedPlugin : IPlugin
{
    private readonly string _raw;
    public string Name => "feed_meta";
    public FeedPlugin()
    {
        var template = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Plugins",
            "feed_template.html"
        );
        _raw = File.ReadAllText(template);
    }

    public string Render(object data)
    {
        var liquidTemplate = Template.ParseLiquid(_raw);
        var context = new LiquidTemplateContext();
        // var ld = LdJson(data);
        var scriptObject = new ScriptObject()
        {
            ["feed_tag"] = data
            //     ["seo_tag"] = data,
            //     ["json_ld"] = ld
        };
        scriptObject.Import(typeof(GlobalFunctions));
        context.PushGlobal(scriptObject);
        var pluginResult = liquidTemplate.Render(context);
        return pluginResult;
    }
}