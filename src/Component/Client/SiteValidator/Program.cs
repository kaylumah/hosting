// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using HtmlAgilityPack;

namespace Kaylumah.Ssg.Client.SiteValidator;

public class MarkupErrors
{
    public string ErrorCode { get; set; }
    public string ErrorReason { get; set; }
}

interface IFilter
{
    bool Validate(HtmlDocument document);
}

class TagFilter : IFilter
{
    private readonly string _path;

    public TagFilter(string path)
    {
        _path = path;
    }

    public bool Validate(HtmlDocument document)
    {
        var result = document.DocumentNode.SelectNodes(_path).SingleOrDefault();
        return result != null;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var rules = new IFilter[] {
                new TagFilter("//meta[@name='description']")
            };



        var path = Path.Combine(Environment.CurrentDirectory, "dist");
        if (Directory.Exists(path))
        {
            var bannedDirectories = new string[] { "NODE_MODULES", "ASSETS" };

            var assets = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => Path.GetDirectoryName(s).ToUpper(CultureInfo.InvariantCulture).Contains("ASSETS"))
                .ToList();

            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => !bannedDirectories.Any(d => Path.GetDirectoryName(s).ToUpper(CultureInfo.InvariantCulture).Contains(d)))
                .ToList();

            var htmlFiles = files.Where(file => ".html".Equals(Path.GetExtension(file)))/*.Take(1)*/.ToList();

            var pageResults = new List<PageLinkResult>();

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

                var errors = new List<MarkupErrors>();
                foreach (var error in document.ParseErrors)
                {
                    errors.Add(new MarkupErrors
                    {
                        ErrorCode = error.Code.ToString(),
                        ErrorReason = error.Reason
                    });
                }

                foreach (var rule in rules)
                {
                    var result = rule.Validate(document);
                    var type = rule.GetType();
                }

                bool hasHead = document.DocumentNode.SelectSingleNode("html/head") != null;
                bool hasBody = document.DocumentNode.SelectSingleNode("html/body") != null;

                var rootNode = document.DocumentNode.SelectSingleNode("html");

                var head = document.DocumentNode.SelectSingleNode("html/head");
                var metaTags = head.SelectNodes("meta");
                var titleTag = head.SelectSingleNode("title");

                var body = document.DocumentNode.SelectSingleNode("html/body");
                var page = new PageLinkResult(html, body);
                pageResults.Add(page);
            }
        }
    }
}

internal class PageLinkResult
{
    private readonly string _fileName;
    private readonly HtmlNode _node;

    public HashSet<string> ExternalAnchors { get; } = new HashSet<string>();
    public HashSet<string> InternalAnchors { get; } = new HashSet<string>();
    public HashSet<string> ExternalImages { get; } = new HashSet<string>();
    public HashSet<string> InternalImages { get; } = new HashSet<string>();

    public PageLinkResult(string fileName, HtmlNode node)
    {
        _fileName = fileName;
        _node = node;
        Process();
    }

    private void Process()
    {
        var anchorTags = _node.SelectNodes("//a[@href]");
        foreach (var tag in anchorTags)
        {
            var attrValue = tag.GetAttributeValue("href", string.Empty);
            if (!string.IsNullOrEmpty(attrValue) && !"#".Equals(attrValue) && !"/".Equals(attrValue))
            {
                if (attrValue.StartsWith("http://") || attrValue.StartsWith("https://"))
                {
                    ExternalAnchors.Add(attrValue);
                }
                else
                {
                    InternalAnchors.Add(attrValue);
                }
            }
        }

        var imageTags = _node.SelectNodes("//img[@src]");
        foreach (var tag in imageTags)
        {
            var attrValue = tag.GetAttributeValue("src", string.Empty);
            if (attrValue.StartsWith("http://") || attrValue.StartsWith("https://"))
            {
                ExternalImages.Add(attrValue);
            }
            else
            {
                InternalImages.Add(attrValue);
            }
        }

    }
}
