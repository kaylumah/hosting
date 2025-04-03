// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class ArticlePublicationMetaData : PublicationMetaData
    {
        public bool SocialShare => GetBoolValue(nameof(SocialShare));
        public bool Feed => GetBoolValue(nameof(Feed));
        public bool Featured => GetBoolValue(nameof(Featured));
        public string CommentId => GetString(nameof(CommentId));

        public string Series
        {
            get
            {
                string result = GetString(nameof(Series));
                return result;
            }
            set
            {
                SetValue(nameof(Series), value);
            }
        }

        public int NumberOfWords
        {
            get
            {
                int result = GetInt(nameof(NumberOfWords));
                return result;
            }
            set
            {
                SetValue(nameof(NumberOfWords), value);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan result = GetTimeSpan(nameof(Duration));
                return result;
            }
            set
            {
                SetValue(nameof(Duration), value);
            }
        }

        public ArticlePublicationMetaData(Dictionary<string, object?> internalData) : base(internalData)
        {
        }
    }
}