// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Markdig;

namespace Kaylumah.Ssg.Utilities
{

    public static class MarkdownUtil
    {
        public static string ToHtml(string source)
        {
            MarkdownPipeline pipeline = BuildPipeline();
            string intermediateResult = Markdown.ToHtml(source, pipeline);
            string result = intermediateResult.Trim();
            return result;
        }

        public static string ToText(string source)
        {
            MarkdownPipeline pipeline = BuildPipeline();
            string intermediateResult = Markdown.ToPlainText(source, pipeline);
            string result = intermediateResult.Trim();
            return result;
        }

        static MarkdownPipeline BuildPipeline()
        {
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/YamlSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/AutoIdentifierSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/PipeTableSpecs.md
            // https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GenericAttributesSpecs.md

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter() // needed to remove any frontmatter
                .UseAutoIdentifiers() // used for clickable headers
                .UsePipeTables() // support for tables
                .UseGenericAttributes() // support for inline attributes (like width, height)
                .Use<MarkdownExtensionAbsoluteLink>()
                .Use<MarkdownExtensionClickableHeaderLink>()
                .Build();
            return pipeline;
        }
    }
}