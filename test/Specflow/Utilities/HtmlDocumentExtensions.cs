// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using HtmlAgilityPack;

namespace Test.Specflow.Utilities;

public static class HtmlDocumentExtensions
{
    public static Dictionary<string, string> ToMetaTags(this HtmlDocument document)
    {
        var nodes = document.DocumentNode.SelectNodes("//meta");
        // var node = document.DocumentNode.SelectSingleNode("//meta[@name='description']");
        // var node2 = document.DocumentNode.SelectSingleNode("//meta[@name='description']//@content");
        // var y = node.Attributes.SingleOrDefault(x => x.Name == "content");
        var dictionary = new Dictionary<string, string>();
        foreach (var node in nodes)
        {
            var keyAttribute = node.Attributes.SingleOrDefault(x => x.Name == "name")
                               ?? node.Attributes.SingleOrDefault(x => x.Name == "property");
            var valueAttribute = node.Attributes.SingleOrDefault(x => x.Name == "content");
            if (keyAttribute != null && valueAttribute != null)
            {
                dictionary.Add(keyAttribute.Value, valueAttribute.Value);
            }
        }
        return dictionary;
    }
}
