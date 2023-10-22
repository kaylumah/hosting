// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using Markdig;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    public static class MarkdownPipelineBuilderExtensions
    {
#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static MarkdownPipelineBuilder UseLinkExtension(this MarkdownPipelineBuilder pipeline)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            OrderedList<IMarkdownExtension> extensions;
            extensions = pipeline.Extensions;

            if (!extensions.Contains<LinkExtension>())
            {
                LinkExtension ext = new LinkExtension();
                extensions.Add(ext);
            }

            return pipeline;
        }
    }

    public class LinkExtension : IMarkdownExtension
    {
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public void Setup(MarkdownPipelineBuilder pipeline)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            pipeline.DocumentProcessed += Pipeline_DocumentProcessed;
        }

        void Pipeline_DocumentProcessed(MarkdownDocument document)
        {
            foreach (MarkdownObject node in document.Descendants())
            {
                if (node is Inline)
                {
                    if (node is LinkInline linkInlineNode)
                    {
                        string uri = linkInlineNode.Url;
                        if (!uri.StartsWith("https://kaylumah.nl", StringComparison.Ordinal))
                        {
                            linkInlineNode.GetAttributes().AddClass("img-fluid");
                        }
                    }
                }
            }
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            // throw new System.NotImplementedException();
        }
    }
}
