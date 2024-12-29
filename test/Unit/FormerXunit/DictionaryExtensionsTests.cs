// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit;

namespace Test.Unit.SnapshotTesting
{
    public class DictionaryExtensionsTests
    {
        [Theory]
        [InlineData("true", true)]
        [InlineData("True", true)]
        [InlineData("false", false)]
        [InlineData("False", false)]
        [InlineData(" ", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void RetrieveBoolValue(string? setValue, bool? expected)
        {
            Dictionary<string, object?> dictionary = new();
            string keyValue = "key";
            dictionary.SetValue(keyValue, setValue);
            bool result = dictionary.GetBoolValue(keyValue);
            Assert.Equal(expected, result);
        }
    }
}