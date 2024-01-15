// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Test.Specflow.Utilities
{
    public static class HtmlDocumentExtensions
    {
        public static List<(string Tag, string Value)> ToMetaTags(this HtmlDocument document)
        {
            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//meta");
            // var node = document.DocumentNode.SelectSingleNode("//meta[@name='description']");
            // var node2 = document.DocumentNode.SelectSingleNode("//meta[@name='description']//@content");
            // var y = node.Attributes.SingleOrDefault(x => x.Name == "content");
            List<(string Tag, string Value)> result = new List<(string Tag, string Value)>();
            foreach (HtmlNode node in nodes)
            {
                HtmlAttribute keyAttribute = node.Attributes.SingleOrDefault(x => x.Name == "name")
                                   ?? node.Attributes.SingleOrDefault(x => x.Name == "property");
                HtmlAttribute valueAttribute = node.Attributes.SingleOrDefault(x => x.Name == "content");
                if (keyAttribute != null && valueAttribute != null)
                {
                    result.Add(new ValueTuple<string, string>(keyAttribute.Value, valueAttribute.Value));
                }
            }

            return result;
        }
    }
}
