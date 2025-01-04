// Copyright (c) Kaylumah, 2025. All rights reserved.
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
            string result = stringBuilder.ToString();
            return result;
        }

        internal static string CreateEmptyXml()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Encoding = Encoding.UTF8;
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
            string result = streamReader.ReadToEnd();
            return result;
        }
        internal static MockFileData EmptyFile()
        {
            MockFileData result = ContentFile(string.Empty);
            return result;
        }

        internal static MockFileData WithFrontMatter(Dictionary<string, object>? data = null)
        {
            string frontMatter = CreateFrontMatter(data);
            MockFileData result = ContentFile(frontMatter);
            return result;
        }

        internal static MockFileData ContentFile(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            MockFileData result = new MockFileData(bytes);
            return result;
        }
    }
}