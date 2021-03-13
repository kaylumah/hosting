using System;
using System.Net;

namespace Kaylumah.Ssg.Utilities.Web
{
    public class Class1
    {
        public string Encode(string input)
        {
            return WebUtility.UrlEncode(input);
        }

        public string Decode(string input)
        {
            return WebUtility.UrlDecode(input);
        }
    }
}
