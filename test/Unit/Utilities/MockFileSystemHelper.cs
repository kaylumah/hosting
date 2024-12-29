// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Xml;

namespace Test.Unit.FormerXunit
{
    public static class MockFileSystemHelper
    {
        internal static string CreateFrontMatter(Dictionary<string, object>? data = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("---");
            if (data != null)
            {
                string raw = new YamlDotNet.Serialization.Serializer().Serialize(data);
                stringBuilder.Append(raw);
            }

            stringBuilder.AppendLine("---");
            return stringBuilder.ToString();
        }

#pragma warning disable IDE0051
        internal static string CreateEmptyXml()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };
            MemoryStream stream = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("feed");
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
            }

            stream.Position = 0;
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
#pragma warning restore IDE0051
        internal static MockFileData EmptyFile()
        {
            return ContentFile(string.Empty);
        }

        internal static MockFileData WithFrontMatter(Dictionary<string, object>? data = null)
        {
            string frontMatter = CreateFrontMatter(data);
            return ContentFile(frontMatter);
        }

        internal static MockFileData ContentFile(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return new MockFileData(bytes);
        }
    }
}