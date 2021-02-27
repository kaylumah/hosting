using System.Diagnostics;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    [DebuggerDisplay("File (Name={Name})")]
    public class File
    {
        public FileMetaData MetaData { get; set; }
        public string Content { get; set; }
        public string Name { get;set; }
    }
}