// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;

namespace Test.E2e
{
    public static partial class VerifierHelper
    {
        //[GeneratedRegex(@"(?<before>https://)(?<val>[a-zA-Z0-9\-\.]*(.net|.nl))(?<after>\/[\w/_-]*\.(html|xml|png|svg))?")]
        [GeneratedRegex(@"(?<before>https://)(?<val>(kaylumah.nl|green-field-0353fee03-[0-9]{3}.westeurope.1.azurestaticapps.net))")]
        public static partial Regex BaseUrl();
    }
}
