using System.Diagnostics;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    [DebuggerDisplay("{Name} {Files.Length} Files")]
    public class FileCollection
    {
        public string Name { get; set; }
        public File[] Files { get; set; }
    }
}