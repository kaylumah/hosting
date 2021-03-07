using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;

namespace Kaylumah.Ssg.Client.SiteValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "dist");
            if (Directory.Exists(path))
            {
                var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList();

                var htmlFiles = files.Where(file => ".html".Equals(Path.GetExtension(file))).ToList();

                // https://html-agility-pack.net/knowledge-base/16645257/how-to-use-html-agility-pack-for-html-validations
                // https://html-agility-pack.net/knowledge-base/2354653/grabbing-meta-tags-and-comments-using-html-agility-pack
                foreach (var html in htmlFiles)
                {
                    Console.WriteLine($"Validating {html}");

                    var document = new HtmlDocument()
                    {
                        OptionFixNestedTags = true
                    };
                    document.LoadHtml(File.ReadAllText(html));

                    var errors = document.ParseErrors;
                    bool hasHead = document.DocumentNode.SelectSingleNode("html/head") != null;
                    bool hasBody = document.DocumentNode.SelectSingleNode("html/body") != null;

                    var rootNode = document.DocumentNode.SelectSingleNode("html");

                    var head = document.DocumentNode.SelectSingleNode("html/head");
                    var metaTags = head.SelectNodes("meta");
                    var titleTag = head.SelectSingleNode("title");
                }
            }
        }
    }
}
