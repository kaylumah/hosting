// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Syndication
{
    public class CDataSyndicationContent : TextSyndicationContent
    {
        public CDataSyndicationContent(TextSyndicationContent content)
            : base(content)
        {}

        protected override void  WriteContentsTo(System.Xml.XmlWriter writer)
        {
            writer.WriteCData(Text);
        }
    }
}
