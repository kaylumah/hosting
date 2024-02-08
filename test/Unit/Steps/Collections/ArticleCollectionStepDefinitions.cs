// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using Reqnroll;
using Test.Unit.Entities;

namespace Test.Unit.Steps.Collections
{
    [Binding]
    public class ArticleCollectionStepDefinitions
    {
        readonly ArticleCollection _ArticleCollection;

        public ArticleCollectionStepDefinitions(ArticleCollection articleCollection)
        {
            _ArticleCollection = articleCollection;
        }

        [Given("the following articles:")]
        public void GivenTheFollowingArticles(ArticleCollection articleCollection)
        {
            _ArticleCollection.AddRange(articleCollection);
        }
    }
}
