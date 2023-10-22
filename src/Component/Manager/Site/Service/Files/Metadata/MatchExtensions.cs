
// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Linq;

namespace System.Text.RegularExpressions
{
    public static class MatchExtensions
    {
        public static string FileNameByPattern(this Match match)
        {
            if (match.Success)
            {
                Group fileName = match.Groups.Cast<Group>().FirstOrDefault(x => "filename".Equals(x.Name, StringComparison.Ordinal));
                Group extension = match.Groups.Cast<Group>().FirstOrDefault(x => "ext".Equals(x.Name, StringComparison.Ordinal));

                if (fileName != null && fileName.Success && extension != null && extension.Success)
                {
                    return $"{fileName}.{extension}";
                }
            }
            return null;
        }

        public static DateTimeOffset? DateByPattern(this Match match)
        {
            if (match.Success)
            {
                Group year = match.Groups.Cast<Group>().FirstOrDefault(x => "year".Equals(x.Name, StringComparison.Ordinal));
                Group month = match.Groups.Cast<Group>().FirstOrDefault(x => "month".Equals(x.Name, StringComparison.Ordinal));
                Group day = match.Groups.Cast<Group>().FirstOrDefault(x => "day".Equals(x.Name, StringComparison.Ordinal));

                if (year != null && year.Success && month != null && month.Success && day != null && day.Success)
                {
                    return DateTimeOffset.Parse($"{year.Value}-{month.Value}-{day.Value}", CultureInfo.InvariantCulture);
                }
            }
            return null;
        }
    }
}
