// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable
namespace VerifyTests
{
    public static class VerifySettingsExtensions
    {
        public static VerifySettings ScrubMatches(this VerifySettings verifySettings, Regex regex, string replacementPrefix = "Val_")
        {
            string[] groupNames = regex.GetGroupNames();

            const string beforeGroupName = "before";
            bool hasBeforeGroup = groupNames.Contains(beforeGroupName);

            const string afterGroupName = "after";
            bool hasAfterGroup = groupNames.Contains(afterGroupName);

            const string valGroupName = "val";
            if (!groupNames.Contains(valGroupName))
            {
                throw new ArgumentException($"Regex must contain a capturing group named {valGroupName}", nameof(regex));
            }

            List<string> foundValues = new List<string>();
            Func<string, string> replacementFunc = val =>
            {
                int index = foundValues.IndexOf(val);
                if (index < 0)
                {
                    index = foundValues.Count;
                    foundValues.Add(val);
                }
                return replacementPrefix + (index + 1).ToString();
            };

            bool TryReplaceMatches(string value, [NotNullWhen(true)] out string? result)
            {
                result = value;
                MatchCollection matches = regex.Matches(result);
                IEnumerable<IGrouping<string, Match>> groupedMatches = matches
                    .Cast<Match>()
                    .GroupBy(m =>
                        (hasBeforeGroup ? m.Groups[beforeGroupName].Value : string.Empty) +
                        m.Groups[valGroupName].Value +
                        (hasAfterGroup ? m.Groups[afterGroupName].Value : string.Empty));
                foreach (IGrouping<string, Match> uniqueValueMatch in groupedMatches)
                {
                    Match match = uniqueValueMatch.First();
                    result = result.Replace(
                        uniqueValueMatch.Key,
                        match.Groups[beforeGroupName].Value +
                        replacementFunc(match.Groups[valGroupName].Value) +
                        match.Groups[afterGroupName].Value);
                }

                return groupedMatches.Any();
            }

            verifySettings.AddScrubber(builder =>
            {
                if (!TryReplaceMatches(builder.ToString(), out string result))
                {
                    return;
                }

                builder.Clear();
                builder.Append(result);
            });

            return verifySettings;
        }
    }
}
