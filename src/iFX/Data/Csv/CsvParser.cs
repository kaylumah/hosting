// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using CsvHelper;
using Kaylumah.Ssg.iFX.Data.Abstractions;

namespace Kaylumah.Ssg.iFX.Data.Csv
{
    public class CsvParser : ICsvParser
    {
        T[] ICollectionParser.Parse<T>(string raw)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(raw);
            Type type = typeof(T);
            bool isDictionary = typeof(Dictionary<string, object>) == type;

#pragma warning disable IDESIGN103 // CsvReaderOptions properties are init-only.
            CsvReaderOptions config = new CsvReaderOptions
            {
                Separator = ';',
                HasHeader = true,
                MalformedData = CsvMalformedDataHandling.Throw,
                MissingField = CsvMissingFieldHandling.Throw
            };
#pragma warning restore IDESIGN103

            if (isDictionary)
            {
                Dictionary<string, object?>[] unknownStructure = ParseDictionary(raw, config);
                T[] records = unknownStructure as T[] ?? [];
                return records;
            }
            else
            {
                T[] records = ParseRecords<T>(raw, config);
                return records;
            }
        }

        T[] ParseRecords<T>(string raw, CsvReaderOptions config)
        {
            List<T> records = new List<T>();
            using CsvReader csvReader = CsvReader.FromText(raw, config);
            IEnumerable<T> internalRecords = csvReader.GetRecords<T>();
            records.AddRange(internalRecords);

            T[] result = records.ToArray();
            return result;
        }

        Dictionary<string, object?>[] ParseDictionary(string raw, CsvReaderOptions config)
        {
            List<Dictionary<string, object?>> records = new List<Dictionary<string, object?>>();
            using CsvReader csv = CsvReader.FromText(raw, config);

            if (0 < csv.Headers.Count)
            {
                while (csv.Read())
                {
                    Dictionary<string, object?> record = new Dictionary<string, object?>();
                    bool isEmptyRecord = true;
                    foreach (string header in csv.Headers)
                    {
                        string? field = csv.GetString(header);
                        if (!string.IsNullOrEmpty(field))
                        {
                            isEmptyRecord = false;
                        }

                        record[header] = field;
                    }

                    if (!isEmptyRecord)
                    {
                        records.Add(record);
                    }
                }
            }

            Dictionary<string, object?>[] result = records.ToArray();
            return result;
        }
    }
}