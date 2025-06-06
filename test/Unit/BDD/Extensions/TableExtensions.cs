﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reqnroll;
using Test.Unit.BDD;
using Test.Unit.Helpers;

namespace Test.Unit.BDD.Extensions
{
    public static class TableExtensions
    {
        static readonly PropertyNameEqualityComparer _PropertyNameEqualityComparer;

        static TableExtensions()
        {
            _PropertyNameEqualityComparer = new();
        }

        public static IEnumerable<string> AsStrings(this Table table, string column)
        {
            string[] result = table.Rows.Select(r => r[column]).ToArray();
            return result;
        }

        public static void ValidateIfMappedCorrectlyTo<TObject>(this Table table) where TObject : class
        {
            _ = table ?? throw new ArgumentNullException(nameof(table));

            string[] tableHeaderNames = table.Header.ToArray();
            PropertyInfo[] objectPropertyInfos = typeof(TObject).GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                                                    BindingFlags.GetProperty |
                                                                    BindingFlags.SetProperty);
            IEnumerable<PropertyInfo> gherkinTableHeaderPropertyInfos = objectPropertyInfos.Where(objectPropertyInfo =>
                objectPropertyInfo.GetCustomAttributes().Any(attribute => attribute is GherkinTableHeaderAttribute));
            PropertyInfo[] orderedGherkinTableHeaderPropertyInfos = gherkinTableHeaderPropertyInfos.OrderBy(
                gherkinTableHeaderPropertyInfo =>
                    gherkinTableHeaderPropertyInfo.GetCustomAttribute<GherkinTableHeaderAttribute>()?.HeaderIndex ??
                    throw new InvalidOperationException(
                        $"PropertyInfo must have declared {nameof(GherkinTableHeaderAttribute)}.")).ToArray();
            string[] orderedGherkinTableHeaderPropertyNames = orderedGherkinTableHeaderPropertyInfos
                .Select(gherkinTableHeaderPropertyInfo => gherkinTableHeaderPropertyInfo.Name).ToArray();

            PropertyInfo[] gherkinTableHeaderPropertyInfosWithDuplicateIndex =
                FindGherkinTableHeaderPropertyInfosWithDuplicateIndex(orderedGherkinTableHeaderPropertyInfos);
            if (gherkinTableHeaderPropertyInfosWithDuplicateIndex.Length != 0)
            {
                IEnumerable<string> gherkinTableHeaderPropertyNamesWithDuplicateIndex =
                    gherkinTableHeaderPropertyInfosWithDuplicateIndex.Select(
                        gherkinTableHeaderPropertyInfoWithDuplicateIndex =>
                            gherkinTableHeaderPropertyInfoWithDuplicateIndex.Name);
                throw new ArgumentException(
                    $"{typeof(TObject).FullName} declares two or more gherkin table headers with a duplicate index. (Headers with duplicate index: {string.Join(", ", gherkinTableHeaderPropertyNamesWithDuplicateIndex)}. Declared table headers (in order): {(orderedGherkinTableHeaderPropertyNames.Length != 0 ? string.Join(", ", orderedGherkinTableHeaderPropertyNames) : "none")})",
                    nameof(table));
            }

            string[] headersNotDeclaredAsGherkinTableHeader =
                FindHeadersNotDeclaredAsGherkinTableHeader(tableHeaderNames, orderedGherkinTableHeaderPropertyNames);
            if (headersNotDeclaredAsGherkinTableHeader.Length != 0)
            {
                throw new ArgumentException(
                    $"{nameof(table)} contains headers not declared as gherkin table header on {typeof(TObject).FullName}. (Headers: {string.Join(", ", headersNotDeclaredAsGherkinTableHeader)}. Declared table headers (in order): {(orderedGherkinTableHeaderPropertyNames.Length != 0 ? string.Join(", ", orderedGherkinTableHeaderPropertyNames) : "none")})",
                    nameof(table));
            }

            string[] gherkinTableHeadersNotDeclaredAsHeader =
                FindGherkinTableHeadersNotDeclaredAsHeader(tableHeaderNames, orderedGherkinTableHeaderPropertyNames);
            if (gherkinTableHeadersNotDeclaredAsHeader.Length != 0)
            {
                throw new ArgumentException(
                    $"{nameof(table)} contains no headers for gherkin table headers declared on {typeof(TObject).FullName}. (Missing headers: {string.Join(", ", gherkinTableHeadersNotDeclaredAsHeader)}. Declared table headers (in order): {(orderedGherkinTableHeaderPropertyNames.Length != 0 ? string.Join(", ", orderedGherkinTableHeaderPropertyNames) : "none")})",
                    nameof(table));
            }

            string[] headersNotAtCorrectIndex =
                FindHeadersNotAtCorrectIndex(tableHeaderNames, orderedGherkinTableHeaderPropertyNames);
            if (headersNotAtCorrectIndex.Length != 0)
            {
                throw new ArgumentException(
                    $"The headers of {nameof(table)} are not in the order specified on {typeof(TObject).FullName}. (Headers: {string.Join(", ", headersNotAtCorrectIndex)}. Declared table headers (in order): {(orderedGherkinTableHeaderPropertyNames.Length != 0 ? string.Join(", ", orderedGherkinTableHeaderPropertyNames) : "none")})",
                    nameof(table));
            }
        }

        static PropertyInfo[] FindGherkinTableHeaderPropertyInfosWithDuplicateIndex(
            PropertyInfo[] gherkinTableHeaderPropertyInfos)
        {
            List<PropertyInfo> gherkinTableHeaderPropertyInfosWithDuplicateIndex = new List<PropertyInfo>();
            foreach (PropertyInfo gherkinTableHeaderPropertyInfo in gherkinTableHeaderPropertyInfos)
            {
                if (1 < gherkinTableHeaderPropertyInfos.Count(gherkinTableHeaderPropertyInfo2 =>
                        gherkinTableHeaderPropertyInfo2.GetCustomAttribute<GherkinTableHeaderAttribute>()
                            ?.HeaderIndex == gherkinTableHeaderPropertyInfo
                            .GetCustomAttribute<GherkinTableHeaderAttribute>()?.HeaderIndex))
                {
                    gherkinTableHeaderPropertyInfosWithDuplicateIndex.Add(gherkinTableHeaderPropertyInfo);
                }
            }

            PropertyInfo[] result = gherkinTableHeaderPropertyInfosWithDuplicateIndex.ToArray();
            return result;
        }

        static string[] FindGherkinTableHeadersNotDeclaredAsHeader(IEnumerable<string> tableHeaderNames,
            IEnumerable<string> gherkinTableHeaderPropertyNames)
        {
            string[] result = gherkinTableHeaderPropertyNames.Where(orderedGherkinTableHeaderPropertyName =>
                    !tableHeaderNames.Contains(orderedGherkinTableHeaderPropertyName, _PropertyNameEqualityComparer))
                .ToArray();
            return result;
        }

        static string[] FindHeadersNotAtCorrectIndex(IEnumerable<string> tableHeaderNames,
            string[] orderedGherkinTableHeaderPropertyNames)
        {
            string[] result = tableHeaderNames.Where((t, i) =>
                !_PropertyNameEqualityComparer.Equals(t, orderedGherkinTableHeaderPropertyNames[i])).ToArray();
            return result;
        }

        static string[] FindHeadersNotDeclaredAsGherkinTableHeader(IEnumerable<string> tableHeaderNames,
            IEnumerable<string> gherkinTableHeaderPropertyNames)
        {
            string[] result = tableHeaderNames.Where(tableHeaderName =>
                !gherkinTableHeaderPropertyNames.Contains(tableHeaderName, _PropertyNameEqualityComparer)).ToArray();
            return result;
        }
    }
}
