using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UADInstaller
{
    public class VersionManager
    {
        public List<FileVersion> FileVersion { get; set; } = new List<FileVersion>();
        public string GlobalVersion { get; set; } = "v0.9.0";
    }

    public class FileVersion
    {
        public string FileName { get; set; }
        public int Version { get; set; }
    }
}
