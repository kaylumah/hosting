﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Reqnroll;

namespace Test.Unit.BDD.Extensions
{
    public static class SpecFlowExtensions
    {
        public static Dictionary<string, object> ToDictionary(this Table table)
        {
            // https://stackoverflow.com/questions/47503580/convert-specflow-table-todictionary
            ArgumentNullException.ThrowIfNull(table);

            if (table.Rows.Count == 0)
            {
                throw new InvalidOperationException("Gherkin data table has no rows");
            }

            if (table.Rows.First().Count != 2)
            {
                throw new InvalidOperationException($@"Gherkin data table must have exactly 2 columns. Columns found: ""{string.Join(@""", """, table.Rows.First().Keys)}""");
            }

            Dictionary<string, object> result = table.Rows.ToDictionary(row => row[0], row => (object)row[1]);
            return result;
        }
    }
}
