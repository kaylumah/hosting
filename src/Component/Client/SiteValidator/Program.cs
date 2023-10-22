// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

sealed class TagFilter : IFilter
{
    private readonly string _path;

    public TagFilter(string path)
    {
        _path = path;
    }

    public bool Validate(HtmlDocument document)
    {
        HtmlNode result = document.DocumentNode.SelectNodes(_path).SingleOrDefault();
        return result != null;
    }
}

sealed class Program
{
    static void Main(string[] args)
    {
        IFilter[] rules = new IFilter[] {
                new TagFilter("//meta[@name='description']")
            };



        string path = Path.Combine(Environment.CurrentDirectory, "dist");
        if (Directory.Exists(path))
        {
            string[] bannedDirectories = new string[] { "NODE_MODULES", "ASSETS" };

            List<string> assets = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => Path.GetDirectoryName(s).ToUpper(CultureInfo.InvariantCulture).Contains("ASSETS"))
                .ToList();

            List<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => !bannedDirectories.Any(d => Path.GetDirectoryName(s).ToUpper(CultureInfo.InvariantCulture).Contains(d)))
                .ToList();

            List<string> htmlFiles = files.Where(file => ".html".Equals(Path.GetExtension(file), StringComparison.Ordinal))/*.Take(1)*/.ToList();

            List<PageLinkResult> pageResults = new List<PageLinkResult>();

            // https://html-agility-pack.net/knowledge-base/16645257/how-to-use-html-agility-pack-for-html-validations
            // https://html-agility-pack.net/knowledge-base/2354653/grabbing-meta-tags-and-comments-using-html-agility-pack
            // https://html-agility-pack.net/knowledge-base/25688847/html-agility-pack-get-all-anchors--href-attributes-on-page
            foreach (string html in htmlFiles)
            {
                Console.WriteLine($"Validating {html}");

                HtmlDocument document = new HtmlDocument()
                {
                    OptionFixNestedTags = true
                };
                document.LoadHtml(File.ReadAllText(html));

                List<MarkupErrors> errors = new List<MarkupErrors>();
                foreach (HtmlParseError error in document.ParseErrors)
                {
                    errors.Add(new MarkupErrors
                    {
                        ErrorCode = error.Code.ToString(),
                        ErrorReason = error.Reason
                    });
                }

                foreach (IFilter rule in rules)
                {
                    bool result = rule.Validate(document);
                    Type type = rule.GetType();
                }

                bool hasHead = document.DocumentNode.SelectSingleNode("html/head") != null;
                bool hasBody = document.DocumentNode.SelectSingleNode("html/body") != null;

                HtmlNode rootNode = document.DocumentNode.SelectSingleNode("html");

                HtmlNode head = document.DocumentNode.SelectSingleNode("html/head");
                HtmlNodeCollection metaTags = head.SelectNodes("meta");
                HtmlNode titleTag = head.SelectSingleNode("title");

                HtmlNode body = document.DocumentNode.SelectSingleNode("html/body");
                PageLinkResult page = new PageLinkResult(html, body);
                pageResults.Add(page);
            }
        }
    }
}

sealed class PageLinkResult
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
        HtmlNodeCollection anchorTags = _node.SelectNodes("//a[@href]");
        foreach (HtmlNode tag in anchorTags)
        {
            string attrValue = tag.GetAttributeValue("href", string.Empty);
            if (!string.IsNullOrEmpty(attrValue) && !"#".Equals(attrValue, StringComparison.Ordinal) && !"/".Equals(attrValue, StringComparison.Ordinal))
            {
                if (attrValue.StartsWith("http://", StringComparison.Ordinal) || attrValue.StartsWith("https://", StringComparison.Ordinal))
                {
                    ExternalAnchors.Add(attrValue);
                }
                else
                {
                    InternalAnchors.Add(attrValue);
                }
            }
        }

        HtmlNodeCollection imageTags = _node.SelectNodes("//img[@src]");
        foreach (HtmlNode tag in imageTags)
        {
            string attrValue = tag.GetAttributeValue("src", string.Empty);
            if (attrValue.StartsWith("http://", StringComparison.Ordinal) || attrValue.StartsWith("https://", StringComparison.Ordinal))
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
