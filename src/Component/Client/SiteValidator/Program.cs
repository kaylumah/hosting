using System;
using System.Collections.Generic;
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
                var bannedDirectories = new string[] { "NODE_MODULES" };
                var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(s => !bannedDirectories.Any(d => Path.GetDirectoryName(s).ToUpper().Contains(d)))
                    .ToList();

                var htmlFiles = files.Where(file => ".html".Equals(Path.GetExtension(file))).ToList();

                // https://html-agility-pack.net/knowledge-base/16645257/how-to-use-html-agility-pack-for-html-validations
                // https://html-agility-pack.net/knowledge-base/2354653/grabbing-meta-tags-and-comments-using-html-agility-pack
                // https://html-agility-pack.net/knowledge-base/25688847/html-agility-pack-get-all-anchors--href-attributes-on-page
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

                    var body = document.DocumentNode.SelectSingleNode("html/body");
                    var links = new HashSet<string>();
                    foreach(var link in body.SelectNodes("//a[@href]"))
                    {
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        links.Add(hrefValue);
                    }
                    var uris = links.Where(x => !string.IsNullOrEmpty(x) && "#" != x && "/" != x).Select(x => {
                        if (x.StartsWith("http://") || x.StartsWith("https://"))
                        {
                            return new Uri(x);
                        }
                        return new Uri(x, UriKind.Relative);
                    }).ToList();

                    var fileUris = uris.Where(x => !x.IsAbsoluteUri);

                   foreach (var fileUri in fileUris)
                   {
                       if (!files.Any(file => file.EndsWith(fileUri.ToString())))
                       {
                           Console.WriteLine($"Failed to find {fileUri}");
                       }
                   }

                    // doc.DocumentNode.SelectNodes("")
                }
            }
        }
    }
}
