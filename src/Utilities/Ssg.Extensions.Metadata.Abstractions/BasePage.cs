// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ssg.Extensions.Metadata.Abstractions
{
    [DebuggerDisplay("PageMetaData '{Uri}'")]
    public abstract class BasePage
    {
        public static implicit operator Dictionary<string, object?>(BasePage page) => page._InternalData;
        readonly Dictionary<string, object?> _InternalData;

        public BasePage(Dictionary<string, object?> internalData)
        {
            _InternalData = internalData;
        }

        #region Indexers
        public object? this[string key]
        {
            get => _InternalData.GetValue<object>(key);
        }
        #endregion

        protected string GetString(string key)
        {
            string? result = _InternalData.GetValue<string>(key);
            // TODO fix NULL suppression
            return result!;
        }

        protected int GetInt(string key)
        {
            int result = _InternalData.GetValue<int>(key);
            return result;
        }

        protected TimeSpan GetTimeSpan(string key)
        {
            TimeSpan result = _InternalData.GetValue<TimeSpan>(key);
            return result;
        }

        protected bool GetBoolValue(string key)
        {
            bool result = _InternalData.GetValue<bool>(key);
            return result;
        }

        protected DateTimeOffset GetDateTimeOffsetValue(string key)
        {
            DateTimeOffset result = _InternalData.GetValue<DateTimeOffset>(key);
            return result;
        }

        protected List<string> GetStringValues(string key)
        {
            IEnumerable<string>? values = _InternalData.GetValues<string>(key);
            List<string> result = values?.ToList() ?? new List<string>();
            return result;
        }

        protected void SetValue(string key, object? value)
        {
            _InternalData.SetValue(key, value);
        }

        public string Uri => GetString(nameof(Uri));

        public string BaseUri => GetString(nameof(BaseUri));

        public Uri CanonicalUri => GetCanonicalUri();

        public string Content
        {
            get
            {
                string result = GetString(nameof(Content));
                return result;
            }
            set
            {
                SetValue(nameof(Content), value);
            }
        }

        public string Type
        {
            get
            {
                string result = GetString(nameof(Type));
                return result;
            }
            set
            {
                SetValue(nameof(Type), value);
            }
        }

        Uri GetCanonicalUri()
        {
            string baseUrl = BaseUri;
            Uri result;
            if ("index.html".Equals(Uri, StringComparison.OrdinalIgnoreCase))
            {
                result = new Uri(baseUrl);
            }
            else
            {
                result = RenderHelperFunctions.AbsoluteUri(baseUrl, Uri);
            }

            return result;
        }
    }
}