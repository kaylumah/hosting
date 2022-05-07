// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Markdig;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities;

public static class MarkdownPipelineBuilderExtensions
{
    public static MarkdownPipelineBuilder UseLinkExtension(this MarkdownPipelineBuilder pipeline)
    {
        OrderedList<IMarkdownExtension> extensions;
        extensions = pipeline.Extensions;


        if (!extensions.Contains<LinkExtension>())
        {
            extensions.Add(new LinkExtension());
        }


        return pipeline;
    }
}

public class LinkExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.DocumentProcessed += Pipeline_DocumentProcessed;
    }

    private void Pipeline_DocumentProcessed(MarkdownDocument document)
    {
        foreach (var node in document.Descendants())
        {
            if (node is Inline)
            {
                if (node is LinkInline linkInlineNode)
                {
                    var uri = linkInlineNode.Url;
                    if (!uri.StartsWith("https://kaylumah.nl"))
                    {
                        linkInlineNode.GetAttributes().AddClass("img-fluid");
                    }
                }
            }
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        // throw new System.NotImplementedException();
    }
}