using System.IO;
using System.Text;

namespace Kaylumah.Ssg.Utilities
{
    public class EncodingUtil
    {
        public Encoding DetermineEncoding(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.Default, detectEncodingFromByteOrderMarks: true);
            if (reader.Peek() >= 0) // you need this!
                reader.Read();

            return reader.CurrentEncoding;
        }
    }
}