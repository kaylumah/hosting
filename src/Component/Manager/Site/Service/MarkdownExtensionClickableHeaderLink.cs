// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Kaylumah.Ssg.Utilities
{
    class MarkdownExtensionClickableHeaderLink : IMarkdownExtension
    {
        void IMarkdownExtension.Setup(MarkdownPipelineBuilder pipeline)
        {
            // Empty on purpose
        }

        void IMarkdownExtension.Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                HeadingRenderer? headingRenderer = htmlRenderer.ObjectRenderers.FindExact<HeadingRenderer>();
                headingRenderer?.TryWriters.Add(TryHeadingBlockRenderer);
            }
        }

        bool TryHeadingBlockRenderer(HtmlRenderer renderer, HeadingBlock headingBlock)
        {
            LinkInline inline = new LinkInline($"#{headingBlock.GetAttributes().Id}", null!);
            ContainerInline previousInline = headingBlock.Inline!;
            headingBlock.Inline = null;
            inline.AppendChild(previousInline);
            headingBlock.Inline = inline;
            // renderer.Write(headingBlock);
            return false;
        }
    }
}