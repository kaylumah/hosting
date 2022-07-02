﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using TechTalk.SpecFlow;

namespace Test.Specflow;

#pragma warning disable CS3001
public static class SpecFlowExtensions
{
    public static Dictionary<string, object> ToDictionary(this Table table)
    {
        // https://stackoverflow.com/questions/47503580/convert-specflow-table-todictionary
        if (table == null)
            throw new ArgumentNullException(nameof(table));

        if (table.Rows.Count == 0)
            throw new InvalidOperationException("Gherkin data table has no rows");

        if (table.Rows.First().Count != 2)
            throw new InvalidOperationException($@"Gherkin data table must have exactly 2 columns. Columns found: ""{string.Join(@""", """, table.Rows.First().Keys)}""");

        return table.Rows.ToDictionary(row => row[0], row => (object)row[1]);
    }
}
#pragma warning restore CS3001
