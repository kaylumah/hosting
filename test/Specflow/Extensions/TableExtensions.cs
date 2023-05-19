// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Reflection;
using Test.Specflow.Helpers;

namespace TechTalk.SpecFlow;

public static class TableExtensions
{
    private static readonly PropertyNameEqualityComparer PropertyNameEqualityComparer = new();

    public static IEnumerable<string> AsStrings(this Table table, string column)
    {
        return table.Rows.Select(r => r[column]).ToArray();
    }

    public static void ValidateIfMappedCorrectlyTo<TObject>(this Table table) where TObject : class
    {
        _ = table ?? throw new ArgumentNullException(nameof(table));

        var tableHeaderNames = table.Header.ToArray();
        var objectPropertyInfos = typeof(TObject).GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                                                BindingFlags.GetProperty |
                                                                BindingFlags.SetProperty);
        var gherkinTableHeaderPropertyInfos = objectPropertyInfos.Where(objectPropertyInfo =>
            objectPropertyInfo.GetCustomAttributes().Any(attribute => attribute is GherkinTableHeaderAttribute));
        var orderedGherkinTableHeaderPropertyInfos = gherkinTableHeaderPropertyInfos.OrderBy(
            gherkinTableHeaderPropertyInfo =>
                gherkinTableHeaderPropertyInfo.GetCustomAttribute<GherkinTableHeaderAttribute>()?.HeaderIndex ??
                throw new InvalidOperationException(
                    $"PropertyInfo must have declared {nameof(GherkinTableHeaderAttribute)}.")).ToArray();
        var orderedGherkinTableHeaderPropertyNames = orderedGherkinTableHeaderPropertyInfos
            .Select(gherkinTableHeaderPropertyInfo => gherkinTableHeaderPropertyInfo.Name).ToArray();

        var gherkinTableHeaderPropertyInfosWithDuplicateIndex =
            FindGherkinTableHeaderPropertyInfosWithDuplicateIndex(orderedGherkinTableHeaderPropertyInfos);
        if (gherkinTableHeaderPropertyInfosWithDuplicateIndex.Any())
        {
            var gherkinTableHeaderPropertyNamesWithDuplicateIndex =
                gherkinTableHeaderPropertyInfosWithDuplicateIndex.Select(
                    gherkinTableHeaderPropertyInfoWithDuplicateIndex =>
                        gherkinTableHeaderPropertyInfoWithDuplicateIndex.Name);
            throw new ArgumentException(
                $"{typeof(TObject).FullName} declares two or more gherkin table headers with a duplicate index. (Headers with duplicate index: {string.Join(", ", gherkinTableHeaderPropertyNamesWithDuplicateIndex)}. Declared table headers (in order): {(orderedGherkinTableHeaderPropertyNames.Any() ? string.Join(", ", orderedGherkinTableHeaderPropertyNames) : "none")})",
                nameof(table));
        }

        var headersNotDeclaredAsGherkinTableHeader =
            FindHeadersNotDeclaredAsGherkinTableHeader(tableHeaderNames, orderedGherkinTableHeaderPropertyNames);
        if (headersNotDeclaredAsGherkinTableHeader.Any())
        {
            throw new ArgumentException(
                $"{nameof(table)} contains headers not declared as gherkin table header on {typeof(TObject).FullName}. (Headers: {string.Join(", ", headersNotDeclaredAsGherkinTableHeader)}. Declared table headers (in order): {(orderedGherkinTableHeaderPropertyNames.Any() ? string.Join(", ", orderedGherkinTableHeaderPropertyNames) : "none")})",
                nameof(table));
        }

        var gherkinTableHeadersNotDeclaredAsHeader =
            FindGherkinTableHeadersNotDeclaredAsHeader(tableHeaderNames, orderedGherkinTableHeaderPropertyNames);
        if (gherkinTableHeadersNotDeclaredAsHeader.Any())
        {
            throw new ArgumentException(
                $"{nameof(table)} contains no headers for gherkin table headers declared on {typeof(TObject).FullName}. (Missing headers: {string.Join(", ", gherkinTableHeadersNotDeclaredAsHeader)}. Declared table headers (in order): {(orderedGherkinTableHeaderPropertyNames.Any() ? string.Join(", ", orderedGherkinTableHeaderPropertyNames) : "none")})",
                nameof(table));
        }

        var headersNotAtCorrectIndex =
            FindHeadersNotAtCorrectIndex(tableHeaderNames, orderedGherkinTableHeaderPropertyNames);
        if (headersNotAtCorrectIndex.Any())
        {
            throw new ArgumentException(
                $"The headers of {nameof(table)} are not in the order specified on {typeof(TObject).FullName}. (Headers: {string.Join(", ", headersNotAtCorrectIndex)}. Declared table headers (in order): {(orderedGherkinTableHeaderPropertyNames.Any() ? string.Join(", ", orderedGherkinTableHeaderPropertyNames) : "none")})",
                nameof(table));
        }
    }

    private static PropertyInfo[] FindGherkinTableHeaderPropertyInfosWithDuplicateIndex(
        PropertyInfo[] gherkinTableHeaderPropertyInfos)
    {
        var gherkinTableHeaderPropertyInfosWithDuplicateIndex = new List<PropertyInfo>();
        foreach (var gherkinTableHeaderPropertyInfo in gherkinTableHeaderPropertyInfos)
        {
            if (gherkinTableHeaderPropertyInfos.Count(gherkinTableHeaderPropertyInfo2 =>
                    gherkinTableHeaderPropertyInfo2.GetCustomAttribute<GherkinTableHeaderAttribute>()
                        ?.HeaderIndex == gherkinTableHeaderPropertyInfo
                        .GetCustomAttribute<GherkinTableHeaderAttribute>()?.HeaderIndex) > 1)
            {
                gherkinTableHeaderPropertyInfosWithDuplicateIndex.Add(gherkinTableHeaderPropertyInfo);
            }
        }

        return gherkinTableHeaderPropertyInfosWithDuplicateIndex.ToArray();
    }

    private static string[] FindGherkinTableHeadersNotDeclaredAsHeader(IEnumerable<string> tableHeaderNames,
        IEnumerable<string> gherkinTableHeaderPropertyNames)
    {
        return gherkinTableHeaderPropertyNames.Where(orderedGherkinTableHeaderPropertyName =>
                !tableHeaderNames.Contains(orderedGherkinTableHeaderPropertyName, PropertyNameEqualityComparer))
            .ToArray();
    }

    private static string[] FindHeadersNotAtCorrectIndex(IEnumerable<string> tableHeaderNames,
        string[] orderedGherkinTableHeaderPropertyNames)
    {
        return tableHeaderNames.Where((t, i) =>
            !PropertyNameEqualityComparer.Equals(t, orderedGherkinTableHeaderPropertyNames[i])).ToArray();
    }

    private static string[] FindHeadersNotDeclaredAsGherkinTableHeader(IEnumerable<string> tableHeaderNames,
        IEnumerable<string> gherkinTableHeaderPropertyNames)
    {
        return tableHeaderNames.Where(tableHeaderName =>
            !gherkinTableHeaderPropertyNames.Contains(tableHeaderName, PropertyNameEqualityComparer)).ToArray();
    }
}
