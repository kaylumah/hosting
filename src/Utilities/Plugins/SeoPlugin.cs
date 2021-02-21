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
            var scriptObject = new ScriptObject
            {
                ["seo_tag"] = data
            };
            scriptObject.Import(typeof(GlobalFunctions));
            context.PushGlobal(scriptObject);
            var pluginResult = liquidTemplate.Render(context);
            return pluginResult;
        }
    }
}