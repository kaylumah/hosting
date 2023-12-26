// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using Xunit;

#pragma warning disable CS3003
namespace Test.Specflow.E2e
{
    [CollectionDefinition(nameof(PlaywrightFixture))]
    public class PlaywrightCollection : ICollectionFixture<PlaywrightFixture>
    {}
}
