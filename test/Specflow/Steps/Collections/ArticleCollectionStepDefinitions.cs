// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using TechTalk.SpecFlow;
using Test.Specflow.Entities;

namespace Test.Specflow.Steps.Collections
{
    [Binding]
    public class ArticleCollectionStepDefinitions
    {
        readonly ArticleCollection _articleCollection;

        public ArticleCollectionStepDefinitions(ArticleCollection articleCollection)
        {
            _articleCollection = articleCollection;
        }

        [Given("the following articles:")]
        public void GivenTheFollowingArticles(ArticleCollection articleCollection)
        {
            _articleCollection.AddRange(articleCollection);
        }
    }
}
