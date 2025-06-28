// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
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

            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.Delimiter = ";";
            config.HasHeaderRecord = true;

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

        T[] ParseRecords<T>(string raw, CsvConfiguration config)
        {
            List<T> records = new List<T>();
            byte[] bytes = Encoding.UTF8.GetBytes(raw);
            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream);
            using CsvReader csvReader = new CsvReader(reader, config);
            IEnumerable<T> internalRecords = csvReader.GetRecords<T>();
            records.AddRange(internalRecords);

            T[] result = records.ToArray();
            return result;
        }

        Dictionary<string, object?>[] ParseDictionary(string raw, CsvConfiguration config)
        {
            List<Dictionary<string, object?>> records = new List<Dictionary<string, object?>>();
            byte[] bytes = Encoding.UTF8.GetBytes(raw);
            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream);
            using CsvReader csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            if (csv.HeaderRecord != null)
            {
                while (csv.Read())
                {
                    Dictionary<string, object?> record = new Dictionary<string, object?>();
                    bool isEmptyRecord = true;
                    foreach (string header in csv.HeaderRecord)
                    {
                        string? field = csv.GetField(header);
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